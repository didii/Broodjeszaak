using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BroodjeszaakLib {
    public class PriceList {
        #region Types
        /// <summary>
        /// An item that has a category, name and price
        /// </summary>
        public class PricedItem {
            public enum EType {
                Brood,
                Beleg,
                Saus,
                Smos
            }

            /// <summary>
            /// The type of the item
            /// </summary>
            public EType Type { get; set; }
            /// <summary>
            /// The name of the item
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// The price of the item
            /// </summary>
            public float Price { get; set; }

            public override string ToString() {
                return Name.ToTitleCase();
            }
        }
        #endregion

        #region Fields
        /// <summary>
        /// In what column the type is
        /// </summary>
        public static int TypeColumn = 0;
        /// <summary>
        /// In what column the name is
        /// </summary>
        public static int NameColumn = 1;
        /// <summary>
        /// In what columns the price is
        /// </summary>
        public static int PriceColumn = 2;
        /// <summary>
        /// The total amount of columns
        /// </summary>
        public const int NumColumns = 3;
        #endregion

        #region Properties
        /// <summary>
        /// All priced items
        /// </summary>
        public List<PricedItem> Items { get; private set; }

        public List<PricedItem> ListOfBread {
            get { return Items.Where(i => i.Type == PricedItem.EType.Brood).ToList(); }
        }

        public List<PricedItem> ListOfSpreads {
            get { return Items.Where(i => i.Type == PricedItem.EType.Beleg).ToList(); }
        }

        public List<PricedItem> ListOfSauces {
            get { return Items.Where(i => i.Type == PricedItem.EType.Saus).ToList(); }
        }
        #endregion

        #region Constructor
        public PriceList() {
            Items = new List<PricedItem>();
        }
        #endregion

        #region Methods
        /// <summary>
        ///     Reads the prices info from the given file. Assumes file is not too big and requires the first column to represent
        ///     the type of item, the second column the item names and the third column to be the item price.
        /// </summary>
        /// <param name="filePath">Where to find the file</param>
        /// <param name="separator">The column separator</param>
        /// <param name="hasHeaderRow">If the first row should be skipped</param>
        /// <returns>Whether or not the reading resulted in errors</returns>
        public bool LoadFromFile(string filePath, char separator = ';', bool hasHeaderRow = true) {
            Items = new List<PricedItem>();
            var text = File.ReadAllLines(filePath);
            var startLine = hasHeaderRow ? 1 : 0;
            var fault = false;
            for (int i = startLine; i < text.Length; i++) {
                var cols = text[i].Split(separator); //split

                //Check number of columns
                if (cols.Length != NumColumns) {
                    fault = true;
                    Console.WriteLine($"[WARNING] This row does not contain exactly ${NumColumns} columns, skipping row: {text[i]}");
                    continue;
                }
                //Check type
                PricedItem.EType type;
                try {
                    type = (PricedItem.EType)Enum.Parse(typeof(PricedItem.EType), cols[TypeColumn].ToTitleCase());
                }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException) {
                    fault = true;
                    Console.WriteLine($"[WARNING] Type could not be parsed, skipping row: {text[i]}");
                    continue;
                }
                //Check price
                float price;
                try {
                    price = float.Parse(cols[PriceColumn]);
                }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException) {
                    fault = true;
                    Console.WriteLine($"[WARNING] Price could not be parsed, skipping row: {text[i]}");
                    continue;
                }
                AddItem(new PricedItem {Type = type, Name = cols[NameColumn], Price = price});
            }
            return !fault;
        }

        public void AddItem(PricedItem item) {
            Items.Add(item);
        }

        public void RemoveItem(PricedItem item) {
            Items.Remove(item);
        }

        public void Clear() {
            Items.Clear();
        }
        #endregion

        #region Operators
        public PricedItem this[PricedItem.EType type, string name] {
            get { return Items.First(i => i.Type == type && i.Name == name); }
            set {
                Items.Remove(this[type, name]);
                AddItem(value);
            }
        }

        public PricedItem this[string name] {
            get { return Items.First(i => i.Name == name); }
            set {
                Items.Remove(this[name]);
                AddItem(value);
            }
        }

        public PricedItem this[PricedItem.EType type] {
            get { return Items.First(i => i.Type == type); }
        }
        #endregion
    }
}
