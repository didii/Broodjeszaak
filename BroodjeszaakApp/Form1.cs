using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BroodjeszaakLib;

namespace BroodjeszaakApp {
    public partial class Form1 : Form {
        #region Fields
        private PriceList _priceList;
        private Order _order;
        private static int _orderNumber = 0;
        #endregion

        #region Constructor
        public Form1(PriceList pricelist) {
            _priceList = pricelist;
            _order = new Order(pricelist);
            _order.PriceChanged += SetPrice;
            InitializeComponent();

            foreach (var bread in pricelist.ListOfBread) {
                var radio = new RadioButton {Text = bread.Name.ToTitleCase()};
                radio.Padding = new Padding(0);
                //radio.Font = new Font(radio.Font, FontStyle.Regular);
                radio.CheckedChanged += BreadSelectors_CheckedChanged;
                flowLayoutPanelKeuzeBrood.Controls.Add(radio);
            }
            listBoxBeleg.Items.AddRange(pricelist.ListOfSpreads.ToArray());
            comboBoxSaus.Items.AddRange(pricelist.ListOfSauces.ToArray());
        }
        #endregion

        #region Methods
        public void SetRadioButtons(List<string> names) { }

        private void SetPrice(float price) {
            textBoxPrice.Text = price.ToString("C");
        }

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

        private void MoveItemAs(ListViewItem item, string status) {
            listViewPendingOrders.Items.Remove(item);
            item.SubItems[columnHeaderPendingStatus.Index].Text = status;
            listViewOrdersDone.Items.Add(item);
        }

        #endregion

        #region Event handlers
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

        private void SpreadSelector_SelectedIndexChanged(object sender, EventArgs e) {
            var box = (ListBox)sender;
            _order.SetSpread(box.SelectedItems.Cast<PriceList.PricedItem>());
        }

        private void SauceSelector_SelectedIndexChanged(object sender, EventArgs e) {
            var box = (ComboBox)sender;
            if (box.SelectedItem is string)
                _order.SetSauce(null);
            else
                _order.SetSauce((PriceList.PricedItem)box.SelectedItem);
        }

        private void VegetablesSelector_CheckedChanged(object sender, EventArgs e) {
            var box = (CheckBox)sender;
            _order.SetVegetables(box.Checked);
        }

        private void buttonPlaceOrder_Click(object sender, EventArgs e) {
            if (!_order.IsValid()) {
                textBoxOrder.Text = "ERROR: Er werd geen broodje geselecteerd";
                return;
            }
            textBoxOrder.Text = _order.ToString();
            var row = new string[] {
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

        private void buttonOrderDone_Click(object sender, EventArgs e) {
            if (listViewPendingOrders.SelectedItems.Count == 0)
                return;
            foreach (ListViewItem sel in listViewPendingOrders.SelectedItems) {
                ((Order)sel.Tag).PrintTicket();
                MoveItemAs(sel, "Done");
            }
        }

        private void buttonCancelOrder_Click(object sender, EventArgs e) {
            if (listViewPendingOrders.SelectedItems.Count == 0)
                return;
            foreach (ListViewItem sel in listViewPendingOrders.SelectedItems)
                MoveItemAs(sel, "Cancelled");
        }


        #endregion


    }
}
