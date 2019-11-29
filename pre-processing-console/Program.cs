﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Newtonsoft.Json;

namespace pre_processing_console
{
    class Program
    {
        private static List<KeyValuePair<int, string>> wordsList = new List<KeyValuePair<int, string>>();

        static string[] ListPDFsLocally(string path) {
            if(!Directory.Exists(path)) {
                throw new Exception(String.Concat("Ops, parece que esta pasta não existe...", path));
            }

            return Directory.GetFiles(path);
        }

        static string ExtractPDFullText(string pdfPath) {
            PdfReader reader = new PdfReader(pdfPath);    
            StringWriter output = new StringWriter();
            output.WriteLine(PdfTextExtractor.GetTextFromPage(reader, 1, new SimpleTextExtractionStrategy()));         

            return output.ToString();
        }

        static string CleanString(string text) => string.Join(" ", text.Split().Where(x => !new string[] { ";", ",", @"\r", @"\t", @"\n", ":" }.Contains(x)));

        static string[] ExtractImportantWordsFromText(string text) {
            List<String> ignoreList = new List<String>() { "na", "no", "da", "do", "de", "para", "e", "o", "a", "-", ":" };

            text = CleanString(text.ToLower());
            text = string.Join(" ", text.Split().Where(x => !ignoreList.Contains(x)));
            text = RemoveDiacritics(text);

            var words = text.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            return words.Except(ignoreList).ToArray();
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
            string pathToReadPDFFiles = (args.Length != 0) ? args[0] : @"C:\Users\Rafael_Heringer\Downloads\_TESTE";
            string[] listOfPDFs;

            // 1. Lista os arquivos PDFs locais
            // Depois será uma lista de arquivos via banco de dados
            // E depois será apenas uma trigger para todo arquivo subido
            listOfPDFs = ListPDFsLocally(pathToReadPDFFiles);
            
            foreach (var pdfPath in listOfPDFs)
            {
                // 2. Extrai o texto por completo
                var fullTextFromPDF = ExtractPDFullText(pdfPath);
                var PDFWords = ExtractImportantWordsFromText(fullTextFromPDF);
                var PDFWordsCounts = new List<Dictionary<int, int>>();

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
                    var pdfDictionaryWordIndex = PDFWordsCounts.Find(x => x.First().Key == globalDictionaryWordIndex.Key);
                    var isInPdfDictionary = pdfDictionaryWordIndex != null;

                    // Se não está, adiciona
                    if(!isInPdfDictionary) {
                        var dict = new Dictionary<int, int>();
                        dict.Add(globalDictionaryWordIndex.Key, 1);
                        PDFWordsCounts.Add(dict);
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

                await firebaseClient.Child(preprocessedDocument.fileId.Replace(".","").Replace(" ","")).PutAsync(JsonConvert.SerializeObject(preprocessedDocument));
            }
          
            var a = globalWordsDictionary;
            
            await firebaseClient.Child("_global-words").PutAsync(JsonConvert.SerializeObject(globalWordsDictionary));

            Console.WriteLine("Hello World!");
        }
    }
}