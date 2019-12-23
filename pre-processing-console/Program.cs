using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using Newtonsoft.Json;
using pre_processing_console.factories;
using pre_processing_console.models;

namespace pre_processing_console
{
    class Program
    {
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
            var pathToReadPDFFiles = (args.Length != 0) ? args[0] : null;

            Console.WriteLine("Hello! Welcome to the basic of Machine Learning code.");
            Console.WriteLine("I'm the developer who is writing this poor code, and don't have idea how AI Works.");
            Console.WriteLine("");

            if (pathToReadPDFFiles == null) 
            {
                Console.WriteLine("Where is your PDF files to test this?");
                pathToReadPDFFiles =  Console.ReadLine();
            }

            var listOfPDFs = pdfFactory.ListPDFsLocally(pathToReadPDFFiles);
            var preprocessedDocuments = new List<PreprocessedDocumentModel>();

            Console.WriteLine("Creating BoW...");
            Console.WriteLine("");

            foreach (var pdfPath in listOfPDFs)
            {
                // Extract PDF text and words
                var fullTextFromPDF     = pdfFactory.ExtractPDFullText(pdfPath);
                var localBag            = new BagOfWordsFactory.LocalBagOfWords(bagOfWordsFactory, fullTextFromPDF);
                var localBagDictionary  = localBag.CreateBag();

                var fileName = pdfPath.Split("\\").Last();
                var preprocessedDocument = new PreprocessedDocumentModel()
                {
                    fileId = fileName,
                    fileName = fileName,
                    fullText = fullTextFromPDF,
                    keywordsCount = localBagDictionary
                };

                preprocessedDocuments.Add(preprocessedDocument);

                Console.WriteLine("Document " + preprocessedDocument.fileName + " processed.");

                // Console.WriteLine(String.Concat("Saving document: ", preprocessedDocument.fileName));
                // await firebaseClient.Child("preprocessed-documents/" + preprocessedDocument.fileId.Replace(".", "").Replace(" ", "")).PutAsync(JsonConvert.SerializeObject(preprocessedDocument));
            }

            //Trainning DATA
            Console.WriteLine("");
            Console.WriteLine("Processing training data...");
            Console.WriteLine("");
            var TFIndex = new TermFrequenceIndexFactory();
            var groups = TFIndex.Train(preprocessedDocuments);

            groups.ForEach(group => {
                Console.WriteLine("Group " + group.Id + ": ");
                group.documents.ForEach(document => {
                    Console.WriteLine("Document: " + document.fileName);
                });
                Console.WriteLine("==================================");
                Console.WriteLine("");
            });

            // await firebaseClient.Child("global-words").PutAsync(JsonConvert.SerializeObject(bagOfWordsFactory.GetGlobalWsordsDictionary()));

            Console.WriteLine("Hello World! You know... enter to exit.");
            Console.ReadLine();
        }
    }
}
