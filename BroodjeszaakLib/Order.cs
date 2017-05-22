using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;

namespace BroodjeszaakLib {
    public class Order {
        #region Fields
        private PriceList.PricedItem _bread;
        private List<PriceList.PricedItem> _spread;
        private PriceList.PricedItem _sauce;
        private PriceList.PricedItem _vegetables;
        #endregion

        #region Events
        public event Action<float> PriceChanged;
        #endregion

        #region Properties
        public PriceList.PricedItem Bread {
            get { return _bread; }
            private set {
                _bread = value;
                PriceChanged?.Invoke(PriceTotal);
            }
        }

        public List<PriceList.PricedItem> Spread {
            get { return _spread; }
            private set {
                _spread = value;
                PriceChanged?.Invoke(PriceTotal);
            }
        }

        public PriceList.PricedItem Sauce {
            get { return _sauce; }
            private set {
                _sauce = value;
                PriceChanged?.Invoke(PriceTotal);
            }
        }

        public PriceList.PricedItem Vegetables {
            get { return _vegetables; }
            private set {
                _vegetables = value;
                PriceChanged?.Invoke(PriceTotal);
            }
        }

        private float PriceBread => _bread?.Price ?? 0f;

        private float PriceSpread => _spread?.Sum(s => s.Price) ?? 0f;

        private float PriceSauce => _sauce?.Price ?? 0f;

        private float PriceVegetables => _vegetables?.Price ?? 0f;

        public PriceList PriceList { get; set; }

        public float PriceTotal {
            get { return PriceBread + PriceSpread + PriceSauce + PriceVegetables; }
        }
        #endregion

        #region Constructor
        public Order() {
            PriceList = new PriceList();
        }

        public Order(PriceList priceList) {
            PriceList = priceList;
        }

        public Order(Order copy) {
            _bread = copy._bread;
            _spread = new List<PriceList.PricedItem>();
            foreach (var spread in copy._spread)
                _spread.Add(spread);
            _sauce = copy._sauce;
            _vegetables = copy._vegetables;
        }
        #endregion

        #region Methods
        public void SetBread(string name) {
            Bread = PriceList.ListOfBread.FirstOrDefault(b => b.Name == name);
        }

        public void SetSpread(PriceList.PricedItem item) {
            Spread = new List<PriceList.PricedItem> {item};
        }

        public void SetSpread(IEnumerable<PriceList.PricedItem> items) {
            if (items == null || !items.Any())
                Spread = null;
            else
                Spread = items.ToList();
        }

        public void SetSauce(PriceList.PricedItem item) {
            Sauce = item;
        }

        public void SetVegetables(bool yes) {
            Vegetables = yes ? PriceList[PriceList.PricedItem.EType.Smos] : null;
        }

        public bool IsValid() {
            return Bread != null;
        }

        public string GetTicketString() {
            if (!IsValid())
                throw new NullReferenceException("Invalid order");
            var builder = new StringBuilder();
            builder.AppendLine("Rekening broodjeszaak");
            builder.AppendLine("=====================");
            builder.AppendLine(DateTime.Now.ToString("G"));
            builder.AppendLine("Bestelling:\t1");
            builder.AppendLine();
            if (Bread != null)
                builder.AppendLine(Bread.Name);
            if (Spread != null && Spread.Count != 0)
                foreach (var spread in Spread)
                    builder.AppendLine(spread.Name);
            if (Sauce != null)
                builder.AppendLine(Sauce.Name);
            if (Vegetables != null)
                builder.AppendLine(Vegetables.Name);
            builder.AppendLine();
            builder.AppendLine("Totale prijs:\t" + PriceTotal);
            return builder.ToString();
        }

        public void PrintTicket() {
            var name = DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
            File.WriteAllText(name, GetTicketString());
            System.Diagnostics.Process.Start(name);
        }

        public override string ToString() {
            var builder = new StringBuilder();
            builder.Append("Een");
            if (Bread != null)
                builder.Append($" {Bread.Name.ToLower()} broodje");
            if (Vegetables != null)
                builder.Append($" {Vegetables.Name.ToLower()}");
            if (Spread != null && Spread.Count != 0)
                builder
                    .Append($" met {Spread.Select(s => s.Name.ToLower()).Aggregate((a, b) => $"{a} en {b}")} als beleg");
            else
                builder.Append(" zonder beleg");
            if (Sauce != null)
                builder.Append($" met {Sauce.Name.ToLower()}");
            builder.AppendLine(".");
            builder.Append($"Kostprijs:\t{PriceTotal:C}");
            return builder.ToString();
        }
        #endregion
    }
}
