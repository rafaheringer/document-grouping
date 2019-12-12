using System.Collections.Generic;
using System.Linq;
using pre_processing_console.models;

namespace pre_processing_console.factories
{
    public class JaccardIndexFactory
    {
        private List<JaccardGroupModel> _formedGroups;
        private double _tolerance;

        public JaccardIndexFactory()
        {
            this._formedGroups = new List<JaccardGroupModel>();
            this._tolerance = 0.2;
        }

        private static double Calc(HashSet<int> hs1, HashSet<int> hs2) => ((double)hs1.Intersect(hs2).Count() / (double)hs1.Union(hs2).Count());

        private static double Calc(List<int> ls1, List<int> ls2) => Calc(new HashSet<int>(ls1), new HashSet<int>(ls2));

        public List<JaccardGroupModel> Train(List<PreprocessedDocumentModel> documents)
        {
            // Criar grupos formados com base na média de aparição das palavras
        }

        public List<JaccardGroupModel> Calc(List<PreprocessedDocumentModel> documents)
        {
            // Passar por todos os grupos já formados

            // Fazer cálculo jaccard pelo grupo para determinar grupo definitivo
        }
    }
}