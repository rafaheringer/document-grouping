# document-grouping

Hello! This project was created by me to learn the basics of Text Classification. My problem was: How I can classify a bunch of documents to extract three important things: the due or paid value, the CNPJ (legal identifier in Brazil for organizations) and the due or paid date. These documents are of infinity types and is not efficient discover where is the data manually.

This repository consists in two projects:

### console app
Made with C# .NET Core, this is a Console App with functions to get, prepare and save the data required to start the machine learning. 

### web app
Made with Angular, the objetive is clarify the Machine Learning process with visual context. This is study purpose, so I'm no preocupied with performance.


## First step: Feature Extraction - Bag of Words
<img src="https://qph.fs.quoracdn.net/main-qimg-4934f0958e121d33717f848230ef664a" width="400" style="float: left; margin-right: 15px;" />

The bag-of-words model is a way of representing text data when modeling text with machine learning algorithms, where a vector represents the frequency of a word in a predefined dictionary of words. Machine learning algorithms cannot work with raw text directly; the text must be converted into numbers. Specifically, vectors of numbers. The model is only concerned with whether known words occur in the document, not where in the document.

A bag-of-words is a representation of text that describes the occurrence of words within a document. It involves two things:
1. A vocabulary of known words.
2. A measure of the presence of known words.

The intuition is that documents are similar if they have similar content. Further, that from the content alone we can learn something about the meaning of the document.

I our code, this is made using the Console Application. It's easy: first create a KeyValuePair with distinct words of all documents (this can be stored in any database. Second, count words of document, removing irrelevants chars (like prepositions, numbers and punctuations), and store the result. You can find all code about this in `factories/BagOfWords.factory.cs` file.

Read more:
- [Gentle Introduction Bag Words Model](https://machinelearningmastery.com/gentle-introduction-bag-words-model/)

## Second: the score


TODO:
- [ ] Study and apply code with bigram approach 
- [ ] Test using terms (sentences) instead of words. I can define sentences using the PDF or OCR regions

------------


References: 
- https://monkeylearn.com/text-classification/
- 