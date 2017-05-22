using System;
using System.Linq;
using System.Windows.Forms;
using BroodjeszaakLib;

namespace BroodjeszaakApp {
    /// <summary>
    ///     Holds the logic of the main form
    /// </summary>
    public partial class Form1 : Form {
        #region Constructor
        /// <summary>
        ///     Only constructor available: it needs a pricelist
        /// </summary>
        /// <param name="pricelist"></param>
        public Form1(PriceList pricelist) {
            // Setup some variables
            _priceList = pricelist;
            _order = new Order(pricelist);
            _order.PriceChanged += SetPrice;

            // Create the form
            InitializeComponent();

            // Add bread radio buttons
            foreach (var bread in pricelist.ListOfBread) {
                var radio = new RadioButton {Text = bread.Name.ToTitleCase()};
                radio.Padding = new Padding(0);
                //radio.Font = new Font(radio.Font, FontStyle.Regular);
                radio.CheckedChanged += BreadSelectors_CheckedChanged;
                flowLayoutPanelKeuzeBrood.Controls.Add(radio);
            }
            // Fill up spread
            listBoxBeleg.Items.AddRange(pricelist.ListOfSpreads.ToArray());
            // Fill up sauces
            comboBoxSaus.Items.AddRange(pricelist.ListOfSauces.ToArray());
        }
        #endregion

        #region Fields
        /// <summary>
        ///     The list of all the prices, not used anymore
        /// </summary>
        private PriceList _priceList;

        /// <summary>
        ///     A single order (the current one)
        /// </summary>
        private readonly Order _order;

        /// <summary>
        ///     The order number (should be moved into <see cref="Order"/>
        /// </summary>
        private static int _orderNumber;
        #endregion

        #region Methods
        /// <summary>
        ///     Sets the price to the given value
        /// </summary>
        /// <param name="price"></param>
        private void SetPrice(float price) {
            textBoxPrice.Text = price.ToString("C");
        }

        /// <summary>
        ///     Reset all fields back to the default values (=nothing selected)
        /// </summary>
        private void ClearChoices() {
            //Clear bread choice
            foreach (var control in flowLayoutPanelKeuzeBrood.Controls) {
                var radio = control as RadioButton;
                if (radio == null)
                    continue;
                radio.Checked = radio.Equals(radioButtonBreadNone);
            }
            //Clear spread choice
            listBoxBeleg.ClearSelected();
            //Clear sauce choice
            comboBoxSaus.SelectedIndex = 0;
            //Clear smos choice
            checkBoxSmos.Checked = false;
        }

        /// <summary>
        ///     Moves an item from <see cref="listViewPendingOrders"/> to <see cref="listViewOrdersDone"/> with the given status.
        /// </summary>
        /// <param name="item">The item to move</param>
        /// <param name="status">The updated status value</param>
        private void MoveItemAs(ListViewItem item, string status) {
            listViewPendingOrders.Items.Remove(item);
            item.SubItems[columnHeaderPendingStatus.Index].Text = status;
            listViewOrdersDone.Items.Add(item);
        }
        #endregion

        #region Event handlers
        /// <summary>
        ///     Called whenever a radio button changed value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BreadSelectors_CheckedChanged(object sender, EventArgs e) {
            var radio = (RadioButton)sender;
            if (!radio.Checked) //ignore if unchecked
                return;

            //if None was selected
            if (radio.Equals(radioButtonBreadNone)) {
                _order.SetBread(null);
                return;
            }

            //all other bread types
            _order.SetBread(radio.Text.ToLower());
        }

        /// <summary>
        ///     Called when the selected spread changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpreadSelector_SelectedIndexChanged(object sender, EventArgs e) {
            var box = (ListBox)sender;
            _order.SetSpread(box.SelectedItems.Cast<PriceList.PricedItem>());
        }

        /// <summary>
        ///     Called when a different sauce is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SauceSelector_SelectedIndexChanged(object sender, EventArgs e) {
            var box = (ComboBox)sender;
            if (box.SelectedItem is string)
                _order.SetSauce(null);
            else
                _order.SetSauce((PriceList.PricedItem)box.SelectedItem);
        }

        /// <summary>
        ///     Called when the value of the vegetables checkbox changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VegetablesSelector_CheckedChanged(object sender, EventArgs e) {
            var box = (CheckBox)sender;
            _order.SetVegetables(box.Checked);
        }

        /// <summary>
        ///     Called when the button 'Plaats bestelling' was pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPlaceOrder_Click(object sender, EventArgs e) {
            if (!_order.IsValid()) {
                textBoxOrder.Text = "ERROR: Er werd geen broodje geselecteerd";
                return;
            }
            textBoxOrder.Text = _order.ToString();
            var row = new[] {
                _orderNumber++.ToString(),
                "ToDo",
                _order.Bread.Name.ToTitleCase(),
                _order.Spread?.Select(s => s.Name.ToTitleCase()).Aggregate((a, b) => a + "," + b) ?? "Geen",
                _order.Sauce?.Name.ToTitleCase() ?? "Geen",
                _order.Vegetables == null ? "Neen" : "Ja",
                _order.PriceTotal.ToString("C")
            };
            var item = new ListViewItem(row) {Tag = new Order(_order)};
            listViewPendingOrders.Items.Add(item);

            //Clean up
            ClearChoices();
        }

        /// <summary>
        ///     Called when the button 'Bestelling uitgevoerd' is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOrderDone_Click(object sender, EventArgs e) {
            if (listViewPendingOrders.SelectedItems.Count == 0)
                return;
            foreach (ListViewItem sel in listViewPendingOrders.SelectedItems) {
                ((Order)sel.Tag).PrintTicket();
                MoveItemAs(sel, "Done");
            }
        }

        /// <summary>
        ///     Called when the button 'Cancel bestelling' is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancelOrder_Click(object sender, EventArgs e) {
            if (listViewPendingOrders.SelectedItems.Count == 0)
                return;
            foreach (ListViewItem sel in listViewPendingOrders.SelectedItems)
                MoveItemAs(sel, "Cancelled");
        }
        #endregion
    }
}
