using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BroodjeszaakLib {
    /// <summary>
    ///     Holds all items, their category and prices
    /// </summary>
    public class PriceList {
        #region Constructor
        /// <summary>
        ///     Default constructor
        /// </summary>
        public PriceList() {
            Items = new List<PricedItem>();
        }
        #endregion

        #region Types
        /// <summary>
        ///     An item that has a category, name and price
        /// </summary>
        public class PricedItem {
            public enum EType {
                Brood,
                Beleg,
                Saus,
                Smos
            }

            /// <summary>
            ///     The type of the item
            /// </summary>
            public EType Type { get; set; }

            /// <summary>
            ///     The name of the item
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            ///     The price of the item
            /// </summary>
            public float Price { get; set; }

            /// <summary>
            ///     Handy string representation of an item, used in listviews etc.
            /// </summary>
            /// <returns></returns>
            public override string ToString() {
                return Name.ToTitleCase();
            }
        }
        #endregion

        #region Fields
        /// <summary>
        ///     In what column the type is
        /// </summary>
        public static int TypeColumn = 0;

        /// <summary>
        ///     In what column the name is
        /// </summary>
        public static int NameColumn = 1;

        /// <summary>
        ///     In what columns the price is
        /// </summary>
        public static int PriceColumn = 2;

        /// <summary>
        ///     The total amount of columns
        /// </summary>
        public const int NumColumns = 3;
        #endregion

        #region Properties
        /// <summary>
        ///     All priced items
        /// </summary>
        public List<PricedItem> Items { get; private set; }

        /// <summary>
        ///     All bread types available
        /// </summary>
        public List<PricedItem> ListOfBread {
            get { return Items.Where(i => i.Type == PricedItem.EType.Brood).ToList(); }
        }

        /// <summary>
        ///     All spreads available
        /// </summary>
        public List<PricedItem> ListOfSpreads {
            get { return Items.Where(i => i.Type == PricedItem.EType.Beleg).ToList(); }
        }

        /// <summary>
        ///     All sauces available
        /// </summary>
        public List<PricedItem> ListOfSauces {
            get { return Items.Where(i => i.Type == PricedItem.EType.Saus).ToList(); }
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
            for (var i = startLine; i < text.Length; i++) {
                var cols = text[i].Split(separator); //split

                //Check number of columns
                if (cols.Length != NumColumns) {
                    fault = true;
                    Console
                        .WriteLine($"[WARNING] This row does not contain exactly ${NumColumns} columns, skipping row: {text[i]}");
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

        /// <summary>
        ///     Adds a single item to the list
        /// </summary>
        /// <param name="item"></param>
        /// <remarks>
        ///     Does not check for duplicates. Any duplicate added cannot be retreived in any way except for clearing the list or
        ///     clearing all duplicates.
        /// </remarks>
        public void AddItem(PricedItem item) {
            Items.Add(item);
        }

        /// <summary>
        ///     Removes a single item from the list
        /// </summary>
        /// <param name="item"></param>
        public void RemoveItem(PricedItem item) {
            Items.Remove(item);
        }

        /// <summary>
        ///     Completely clears the list
        /// </summary>
        public void Clear() {
            Items.Clear();
        }
        #endregion

        #region Operators
        /// <summary>
        ///     Gets the first item matching the type and name
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public PricedItem this[PricedItem.EType type, string name] {
            get { return Items.First(i => i.Type == type && i.Name == name); }
            set {
                Items.Remove(this[type, name]);
                AddItem(value);
            }
        }

        /// <summary>
        ///     Gets the first item matching the name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public PricedItem this[string name] {
            get { return Items.First(i => i.Name == name); }
            set {
                Items.Remove(this[name]);
                AddItem(value);
            }
        }

        /// <summary>
        ///     Gets the first item of this type (only used for Smos)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public PricedItem this[PricedItem.EType type] {
            get { return Items.First(i => i.Type == type); }
        }
        #endregion
    }
}
