using System;
using System.Collections.Generic;
using System.Linq;
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
        private static BagOfWordsFactory bagOfWordsFactory = new BagOfWordsFactory();
        private static FirebaseClient firebaseClient = FirebaseAuthentication();

        static FirebaseClient FirebaseAuthentication()
        {
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
            var pathToReadPDFFiles = (args.Length != 0) ? args[0] : @"D:\_teste2";
            var listOfPDFs = pdfFactory.ListPDFsLocally(pathToReadPDFFiles);

            foreach (var pdfPath in listOfPDFs)
            {
                // Extract PDF text and words
                var fullTextFromPDF     = pdfFactory.ExtractPDFullText(pdfPath);
                var localBag            = new BagOfWordsFactory.LocalBagOfWords(bagOfWordsFactory, fullTextFromPDF);
                var localBagDictionary  = localBag.CreateBag();

                // Persinst data
                var fileName = pdfPath.Split("\\").Last();
                var preprocessedDocument = new PreprocessedDocumentModel()
                {
                    fileId = fileName,
                    fileName = fileName,
                    fullText = fullTextFromPDF,
                    keywordsCount = localBagDictionary
                };

                Console.WriteLine(String.Concat("Salvando documento: ", preprocessedDocument.fileName));
                await firebaseClient.Child("preprocessed-documents/" + preprocessedDocument.fileId.Replace(".", "").Replace(" ", "")).PutAsync(JsonConvert.SerializeObject(preprocessedDocument));
            }

            await firebaseClient.Child("global-words").PutAsync(JsonConvert.SerializeObject(bagOfWordsFactory.GetGlobalWordsDictionary()));

            Console.WriteLine("Hello World!");
        }
    }
}
