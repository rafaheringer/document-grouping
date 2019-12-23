using System;
using System.Collections.Generic;
using System.Linq;
using pre_processing_console.models;

namespace pre_processing_console.factories
{
    public class JaccardIndexFactory
    {
        private List<JaccardGroupTrainingModel> _formedGroups;
        private double _tolerance;
        private int _wordsLimit;
        private int _recalculateEvery;

        public JaccardIndexFactory()
        {
            this._formedGroups = new List<JaccardGroupTrainingModel>();
            this._tolerance = 0.2;
            this._wordsLimit = 50;
            this._recalculateEvery = 20;
        }

        private static double Calc(HashSet<int> hs1, HashSet<int> hs2) => ((double)hs1.Intersect(hs2).Count() / (double)hs1.Union(hs2).Count());

        private static double Calc(List<int> ls1, List<int> ls2) => Calc(new HashSet<int>(ls1), new HashSet<int>(ls2));

        private void RecalcGroup(Guid groupId)
        {
            var group = this._formedGroups.Find(x => x.Id == groupId);

            if (group.documents.Count % this._recalculateEvery == 0 || group.averageWordsCount == null)
            {
                var averageWordsCount = new List<KeyValuePair<int, int>>();

                averageWordsCount = group.documents.First().keywordsCount;

                // group.documents.ForEach(document => {
                //     document.keywordsCount.ForEach(wordIndex => {

                //     });
                // });

                group.averageWordsCount = averageWordsCount;
            }
        }

        private void AddDocumentToGroup(PreprocessedDocumentModel document, Guid groupId) => this._formedGroups.Find(x => x.Id == groupId).documents.Add(document);

        private Guid CreateGroup()
        {
            var group = new JaccardGroupTrainingModel()
            {
                averageWordsCount = null,
                Id = Guid.NewGuid(),
                keywordsLimit = this._wordsLimit,
                tolerance = this._tolerance,
                documents = new List<PreprocessedDocumentModel>()
            };

            this._formedGroups.Add(group);

            return group.Id;
        }

        public List<JaccardGroupTrainingModel> Train(List<PreprocessedDocumentModel> documents)
        {
            documents.ForEach(document =>
            {
                document.keywordsCount = document.keywordsCount.OrderByDescending(x => x.Value).Take(this._wordsLimit).ToList();
                var isCompatibleWithAnyGroup = false;

                this._formedGroups.TakeWhile(group =>
                {
                    var result = this.Process(document, group);

                    if (result.isCompatible)
                    {
                        this.AddDocumentToGroup(document, group.Id);
                        this.RecalcGroup(group.Id);
                        isCompatibleWithAnyGroup = true;
                    }

                    return result.isCompatible == false;
                }).ToList();

                if(isCompatibleWithAnyGroup == false) 
                {
                    var groupId = this.CreateGroup();
                    this.AddDocumentToGroup(document, groupId);
                    this.RecalcGroup(groupId);
                }
            });

            return this._formedGroups;
        }

        private JaccardGroupProcessResultModel Process(PreprocessedDocumentModel document, JaccardGroupTrainingModel group)
        {
            double finalSkore = 0;
            var skores = new List<Tuple<double, double>>();
            var groupWordsCount = group.averageWordsCount.Sum(a => a.Value);

            group.averageWordsCount.ForEach(averageWord =>
            {
                double keyGroupAverageCount = averageWord.Value;
                double keyGroupWeight = keyGroupAverageCount / groupWordsCount;
                double skore = 0;

                // Check if the document have this word
                if (document.keywordsCount.Any(d => d.Key == averageWord.Key))
                {
                    var keyCount = document.keywordsCount.Find(d => d.Key == averageWord.Key).Value;
                    var maxTolerance = keyGroupAverageCount + (keyGroupAverageCount * group.tolerance);
                    var minTolerance = keyGroupAverageCount - (keyGroupAverageCount * group.tolerance);

                    skore = 1 - (group.tolerance / Math.Abs(1 - (keyCount / keyGroupAverageCount)));
                    if(double.IsInfinity(skore))
                        skore = 1;

                    else if (skore < 0)
                        skore = 0;
                }

                skores.Add(new Tuple<double, double>(skore, keyGroupWeight));
            });


            skores.ForEach(skore =>
            {
                finalSkore += skore.Item1 * skore.Item2;
            });

            return new JaccardGroupProcessResultModel()
            {
                calculatedSkore = finalSkore,
                isCompatible = finalSkore >= 1 - this._tolerance
            };
        }
    }
}