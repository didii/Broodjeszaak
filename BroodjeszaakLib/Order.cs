using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace BroodjeszaakLib {
    /// <summary>
    ///     A single order holding all items that are used.
    /// </summary>
    /// <remarks>
    ///     This could be improved upon by not holding 4 variables that hold the same item, but to only use a list and do some
    ///     checks for it. The current method was chosen because of time contraints.
    /// </remarks>
    public class Order {
        #region Events
        /// <summary>
        ///     Fired whenever an item was changed in the order
        /// </summary>
        public event Action<float> PriceChanged;
        #endregion

        #region Fields
        /// <summary>
        ///     The type of bread used
        /// </summary>
        private PriceList.PricedItem _bread;

        /// <summary>
        ///     The type(s) (if any) of spread for in-between the bread
        /// </summary>
        private List<PriceList.PricedItem> _spread;

        /// <summary>
        ///     Sauce (if any) to add some flavour
        /// </summary>
        private PriceList.PricedItem _sauce;

        /// <summary>
        ///     The type of vegetables (if any) that are added
        /// </summary>
        private PriceList.PricedItem _vegetables;
        #endregion

        #region Properties
        /// <summary>
        ///     The type of bread used. Fires <see cref="PriceChanged"/> if changed.
        /// </summary>
        public PriceList.PricedItem Bread {
            get { return _bread; }
            private set {
                _bread = value;
                PriceChanged?.Invoke(PriceTotal);
            }
        }

        /// <summary>
        ///     The type(s) (if any) of spread for in-between the bread. Fires <see cref="PriceChanged"/> if changed.
        /// </summary>
        public List<PriceList.PricedItem> Spread {
            get { return _spread; }
            private set {
                _spread = value;
                PriceChanged?.Invoke(PriceTotal);
            }
        }

        /// <summary>
        ///     Sauce (if any) to add some flavour. Fires <see cref="PriceChanged"/> if changed.
        /// </summary>
        public PriceList.PricedItem Sauce {
            get { return _sauce; }
            private set {
                _sauce = value;
                PriceChanged?.Invoke(PriceTotal);
            }
        }

        /// <summary>
        ///     The type of vegetables (if any) that are added. Fires <see cref="PriceChanged"/> if changed.
        /// </summary>
        public PriceList.PricedItem Vegetables {
            get { return _vegetables; }
            private set {
                _vegetables = value;
                PriceChanged?.Invoke(PriceTotal);
            }
        }

        /// <summary>
        ///     Current price of the bread (0 if none selected)
        /// </summary>
        private float PriceBread => _bread?.Price ?? 0f;

        /// <summary>
        ///     Current price for the spread(s) (0 if none selected)
        /// </summary>
        private float PriceSpread => _spread?.Sum(s => s.Price) ?? 0f;

        /// <summary>
        ///     Current price for the sauce (0 if none selected)
        /// </summary>
        private float PriceSauce => _sauce?.Price ?? 0f;

        /// <summary>
        ///     The price for the vegetables (0 if none selected)
        /// </summary>
        private float PriceVegetables => _vegetables?.Price ?? 0f;

        /// <summary>
        ///     The pricelist of all items
        /// </summary>
        public PriceList PriceList { get; set; }

        /// <summary>
        ///     The total price of all items
        /// </summary>
        public float PriceTotal => PriceBread + PriceSpread + PriceSauce + PriceVegetables;
        #endregion

        #region Constructor
        /// <summary>
        ///     Default constructor
        /// </summary>
        public Order() {
            PriceList = new PriceList();
        }

        /// <summary>
        ///     Constructor setting the pricelist
        /// </summary>
        /// <param name="priceList"></param>
        public Order(PriceList priceList) {
            PriceList = priceList;
        }

        /// <summary>
        ///     Deep copies an order
        /// </summary>
        /// <param name="copy"></param>
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
        /// <summary>
        ///     Sets the bread by name (should be by item, but I didn't know you could add an item with a Tag until a bit later)
        /// </summary>
        /// <param name="name"></param>
        public void SetBread(string name) {
            Bread = PriceList.ListOfBread.FirstOrDefault(b => b.Name == name);
        }

        /// <summary>
        ///     Set the spread used. Overwrites all existing ones
        /// </summary>
        /// <param name="item"></param>
        public void SetSpread(PriceList.PricedItem item) {
            Spread = new List<PriceList.PricedItem> {item};
        }

        /// <summary>
        ///     Sets multiple spreads. Overwrites all existing ones
        /// </summary>
        /// <param name="items"></param>
        public void SetSpread(IEnumerable<PriceList.PricedItem> items) {
            if (items == null || !items.Any())
                Spread = null;
            else
                Spread = items.ToList();
        }

        /// <summary>
        ///     Set the sauce to use
        /// </summary>
        /// <param name="item"></param>
        public void SetSauce(PriceList.PricedItem item) {
            Sauce = item;
        }

        /// <summary>
        ///     Set the vegetables to use
        /// </summary>
        /// <param name="yes"></param>
        public void SetVegetables(bool yes) {
            Vegetables = yes ? PriceList[PriceList.PricedItem.EType.Smos] : null;
        }

        /// <summary>
        ///     If the order is valid. Currently only checks if any bread was used
        /// </summary>
        /// <returns></returns>
        public bool IsValid() {
            return Bread != null;
        }

        /// <summary>
        ///     Returns a string representing a ticket
        /// </summary>
        /// <returns></returns>
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
            Process.Start(name);
        }

        /// <summary>
        ///     String representation of this order as a fancy and nice string.
        /// </summary>
        /// <returns></returns>
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
