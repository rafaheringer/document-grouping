﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database;
using Newtonsoft.Json;
using pre_processing_console.factories;

namespace pre_processing_console
{
    class Program
    {
        private static List<KeyValuePair<int, string>> wordsList = new List<KeyValuePair<int, string>>();
        private static PDFFactory pdfFactory = new PDFFactory();

        static string CleanString(string text) => string.Join(" ", text.Split().Where(x => !new string[] { @"\r", @"\t", @"\n" }.Contains(x)));

        static string RemoveNonAlphaFromString(string text) {
            char[] arr = text.Where(c => (char.IsLetter(c) || char.IsWhiteSpace(c))).ToArray(); 
            return new string(arr);
        }

        static string RemoveOneOrTwoLetterFromString(string text) => String.Join(" ", text.Split(" ").Where(x => x.Length > 2));

        static string RemoveKeyWordsFromString(string text) {
            List<String> ignoreList = new List<String>() { "cnpj", "ltda", "mei", "me", "na", "no", "da", "do", "de", "para", "e", "o", "a", "ou"};
            return string.Join(" ", text.Split(" ").Where(x => !ignoreList.Contains(x)));
        }

        static string[] ExtractImportantWordsFromText(string text) {
            text = CleanString(text.ToLower());
            text = RemoveNonAlphaFromString(text);
            text = RemoveKeyWordsFromString(text);
            text = RemoveDiacritics(text);
            text = RemoveOneOrTwoLetterFromString(text);

            return text.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        }

        static string RemoveDiacritics(string text) {
            text = text.Normalize(NormalizationForm.FormD);
            var chars = text.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
            return new string(chars).Normalize(NormalizationForm.FormC);
        }

        static List<KeyValuePair<int, string>> GetWordsDictionary() => wordsList; // TODO: Buscar essa lista em algum banco de dados

        static void AddWordsToDictionary(List<string> words) => words.ForEach(AddWordsToDictionary);

        static void AddWordsToDictionary(string word) => wordsList.Add(new KeyValuePair<int, string>(wordsList.Count, word)); // TODO: Salvar em banco e escolher um método para poder salvar índices de forma paralela.

        static FirebaseClient FirebaseAuthentication() {
            var auth = "QINTF3PV0sjAJJCkjrGNytOkIqPwX2qElwP2PGrA"; // APP Secrect for Testing Porpousing...
            var firebaseClient = new FirebaseClient(
            "https://grouping-documents.firebaseio.com",
            new FirebaseOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(auth) 
            });

            return firebaseClient;
        }

        static async Task Main(string[] args)
        {
            var globalWordsDictionary = GetWordsDictionary();
            var firebaseClient = FirebaseAuthentication();
            string pathToReadPDFFiles = (args.Length != 0) ? args[0] : @"D:\_teste2";
            string[] listOfPDFs;

            // 1. Lista os arquivos PDFs locais
            // Depois será uma lista de arquivos via banco de dados
            // E depois será apenas uma trigger para todo arquivo subido
            listOfPDFs = pdfFactory.ListPDFsLocally(pathToReadPDFFiles);
            
            foreach (var pdfPath in listOfPDFs)
            {
                // 2. Extrai o texto por completo
                var fullTextFromPDF = pdfFactory.ExtractPDFullText(pdfPath);
                var PDFWords = ExtractImportantWordsFromText(fullTextFromPDF);
                var PDFWordsCounts = new List<KeyCountModel>();

                // 3. Compara índice de palvras já configuradas e coloca no índice novas palavras 
                // Isso serve para tornar o processo rápido. Caso contrário, teria que bater palavra por palavra com o modelo, comparando strings
                // TODO: Fazer cálculo diferente para adicionar no índice 
                foreach (var word in PDFWords) {

                    // Verifica se a palavra está no dicionário global
                    var globalDictionaryWordIndex = globalWordsDictionary.Find(x => x.Value == word);
                    var isInGlobalDictionary = globalDictionaryWordIndex.Value != null;

                    // Se não está no dicionário global, adiciona e resgata o índice
                    if(!isInGlobalDictionary) {
                        // AddWordsToDictionary(word);
                        var i = globalWordsDictionary.Count;
                        globalWordsDictionary.Add(new KeyValuePair<int, string>(i, word));
                        globalDictionaryWordIndex = globalWordsDictionary.Find(x => x.Key == i);
                        isInGlobalDictionary = true;
                    }

                    // Verifica se o índice está no dicionário do PDF
                    var pdfDictionaryWordIndex = PDFWordsCounts.Find(x => x.key == globalDictionaryWordIndex.Key);
                    var isInPdfDictionary = pdfDictionaryWordIndex != null;

                    // Se não está, adiciona
                    if(!isInPdfDictionary) {
                        PDFWordsCounts.Add(new KeyCountModel() {
                            key = globalDictionaryWordIndex.Key,
                            count = 1
                        });
                    } else {
                        pdfDictionaryWordIndex.count++;
                    }
                };

                // 5. Salva no banco o metadado deste arquivo (id, nome, texto extraído, índice de palavras e contagem)
                var fileName = pdfPath.Split("\\").Last();
                var preprocessedDocument = new PreprocessedDocumentModel() {
                    fileId = fileName,
                    fileName = fileName,
                    fullText = fullTextFromPDF,
                    keywordsCount = PDFWordsCounts
                };

                Console.WriteLine(String.Concat("Salvando documento: ", preprocessedDocument.fileName));
                await firebaseClient.Child("preprocessed-documents/" + preprocessedDocument.fileId.Replace(".","").Replace(" ","")).PutAsync(JsonConvert.SerializeObject(preprocessedDocument));
            }
          
            var a = globalWordsDictionary;
            
            await firebaseClient.Child("global-words").PutAsync(JsonConvert.SerializeObject(globalWordsDictionary));

            Console.WriteLine("Hello World!");
        }
    }
}
