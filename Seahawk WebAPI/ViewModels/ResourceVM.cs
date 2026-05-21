using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace SeaHawkServices.Web.ViewModels
{
    public class ResourceVM
    {
        public string CurrentTab { get; set; }

        /*     FuelEnergyCalculationVM */
        public string FuelType { get; set; }
        public string file { get; set; }
        public double Density { get; set; }
        public double WaterContent { get; set; }
        public double AshContent { get; set; }
        public double SulfurContent { get; set; }
        public double PricePerMT { get; set; }
        public double GrossSpecificEnergy { get; set; }
        public double NetSpecificEnergy { get; set; }
        public double GrossBtulb { get; set; }
        public double NetBtulb { get; set; }
        public double GrossBtugal { get; set; }
        public double NetBtugal { get; set; }
        public void Calculate()
        {
            // Common calculations
            var red = Math.Round(((141.5 / (Density / 1000)) - 131.5), 3);
            var temp = 0.7461289
                + (0.3473741 * Math.Log10(red))
                - (0.1815079 * Math.Log10(red) * Math.Log10(red));
            var gal = Math.Round(Math.Pow(10, temp), 3);

            if (FuelType?.Equals("Residual", StringComparison.OrdinalIgnoreCase) == true)
            {
                GrossSpecificEnergy = (52.19 - 8.802 * Math.Pow(Density, 2) * 0.000001)
                    * (1 - 0.01 * (WaterContent + AshContent + SulfurContent))
                    + 0.094 * SulfurContent;

                NetSpecificEnergy = ((46.704 - 8.802 * Math.Pow(Density, 2) * 0.000001)
                    + 3.167 * Density * 0.001)
                    * (1 - 0.01 * (WaterContent + AshContent + SulfurContent))
                    + 0.0942 * SulfurContent
                    - 0.02449 * WaterContent;
            }
            else // Distillate
            {
                GrossSpecificEnergy = (51.916 - 8.792 * Math.Pow(Density, 2) * 0.000001)
                    * (1 - 0.01 * (WaterContent + AshContent + SulfurContent))
                    + 0.0942 * SulfurContent;

                NetSpecificEnergy = ((46.423 - 8.792 * Math.Pow(Density, 2) * 0.000001)
                    + 3.17 * Density * 0.001)
                    * (1 - 0.01 * (WaterContent + AshContent + SulfurContent))
                    + 0.0942 * SulfurContent
                    - 0.02449 * WaterContent;
            }

            GrossSpecificEnergy = Math.Round(GrossSpecificEnergy, 3);
            NetSpecificEnergy = Math.Round(NetSpecificEnergy, 3);

            PricePerMT = Math.Round(PricePerMT / NetSpecificEnergy, 3);

            GrossBtulb = Math.Round(GrossSpecificEnergy / 0.002326, 0);
            NetBtulb = Math.Round(NetSpecificEnergy / 0.002326, 0);

            GrossBtugal = Math.Round(GrossBtulb * gal, 0);
            NetBtugal = Math.Round(NetBtulb * gal, 0);
        }
        /* CCAICalculationVM */
        public double KinematicViscosity { get; set; }

        public double CCAI { get; set; }
        public double CII { get; set; }

        public void CalculateCCAI()
        {
            //my condition
            if (Density != 0 && KinematicViscosity != 0)
            {
                var cii = Math.Round((270.795 + (0.1038 * 50)) - (0.254565 * Density) + (23.708 * Math.Log10(Math.Log10(KinematicViscosity + 0.7))));
                var ccai = Math.Round((Density) - 81 - (141 * Math.Log10(Math.Log10(KinematicViscosity + 0.85))) - (483 * Math.Log10((50 + 273) / 323)));
                CCAI = Math.Round(ccai, 1);
                CII = Math.Round(cii, 1);
            }

            //code written on old project
            //if (p == 0 && v == 0)
            //    {
            //        document.getElementById('ccai').value = '';
            //    }
            //    else
            //    {
            //        var cii = Math.round((270.795 + (0.1038 * 50)) - (0.254565 * p) + (23.708 * lg(lg(v + 0.7))));
            //        var ccai = Math.round((p) - 81 - (141 * lg(lg(v + 0.85))) - (483 * lg((50 + 273) / 323)));
            //        document.getElementById('ccai').value = DecRound(ccai);
            //        document.getElementById('cii').value = DecRound(cii);
            //    }

            //converted code 
            //if (ViewModel.Density == 0 && ViewModel.KinematicViscosity == 0)
            //{
            //    ViewModel.CCAI = 0.00; // ''
            //}
            //else
            //{
            //    var cii = Math.Round((270.795 + (0.1038 * 50)) - (0.254565 * ViewModel.Density) + (23.708 * Math.Log10(Math.Log10(ViewModel.KinematicViscosity + 0.7))));
            //    var ccai = Math.Round((ViewModel.Density) - 81 - (141 * Math.Log10(Math.Log10(ViewModel.KinematicViscosity + 0.85))) - (483 * Math.Log10((50 + 273) / 323)));
            //    ViewModel.CCAI = Math.Round(ccai, 1);
            //    ViewModel.CII = Math.Round(cii, 1);
            //}

        }

        /* TempratureConversion */

        public int SelectedTempratureUnit { get; set; }
        public double Celsius { get; set; }
        public double Fahrenheit { get; set; }
        public double ValueToConvert { get; set; }
        public List<SelectListItem> TempratureDDL()
        {
            var ddl = new List<SelectListItem>()
               {
                    new SelectListItem() { Text = "Choose unit..", Value = "0"},
                    new SelectListItem() { Text = "Celsius", Value = "1"},
                    new SelectListItem() { Text = "Fahrenheit", Value = "2"},
              };
            return ddl;
        }


        public void ConvertTemperature()
        {
            if (SelectedTempratureUnit == 1) // from Celsius
            {
                Celsius = Math.Round(ValueToConvert, 2);
                Fahrenheit = Math.Round((ValueToConvert * 1.8) + 32, 2);
            }
            else if (SelectedTempratureUnit == 2) // from Fahrenheit
            {
                Celsius = Math.Round((ValueToConvert - 32) / 1.8, 2);
                Fahrenheit = Math.Round(ValueToConvert, 2);
            }
            else if (SelectedTempratureUnit == 3) // from Kelvin
            {
                Celsius = Math.Round(ValueToConvert - 273, 5);
                Fahrenheit = Math.Round(((ValueToConvert - 273) * (9.0 / 5.0)) + 32, 5);
            }
        }

        /* Sulfur Change-Over Calculation */

        [Display(Name = "Low Sulfur content, % m/m")]
        [Range(0, 1)]
        public decimal LowSulfurContent { get; set; } = 0.000m;

        [Display(Name = "High Sulfur content, % m/m")]
        [Range(0, 1)]
        public decimal HighSulfurContent { get; set; } = 1.000m;

        [Display(Name = "Fuel Oil Quantity in the Service System (mt)")]
        [Range(0, 100000)]
        public decimal FuelOilQty { get; set; } = 0.00m;

        [Display(Name = "Total Fuel Consumption Rate (ton/hr)")]
        [Range(0, 100000)]
        public decimal TotalFuelConsumptionRate { get; set; } = 0.00m;

        [Display(Name = "Required Sulfur content at engine inlet, % m/m")]
        [Range(0, 1)]
        public decimal SelectedValueForRequiredSulfurContent { get; set; } = 0.500m;

        // Step size (legacy JS uses 0.10 hr)
        public decimal CalculationStepHours { get; set; } = 0.10m;
        public string EmailTo { get; set; }
        public string EmailFrom { get; set; }

        public string TimeRequired { get; private set; } = "0.00";
        public string ErrorMessage { get; private set; } = "";
        public string SulphurErrorMessage { get; set; } = "";


        public IEnumerable<SelectListItem> RequiredSulfurContentDDL()
        {
            var current = SelectedValueForRequiredSulfurContent.ToString("0.000", CultureInfo.InvariantCulture);
            return new List<SelectListItem>
        {
            new SelectListItem { Text = "0.500", Value = "0.500", Selected = current == "0.500" },
            new SelectListItem { Text = "0.100", Value = "0.100", Selected = current == "0.100" }
        };
        }

        public void Compute()
        {
            ErrorMessage = "";
            TimeRequired = "0.00";

            if (LowSulfurContent >= 0.10m)
            {
                ErrorMessage = "Sulfur content of fuel to be mixed must be less than 0.10%.";
                return;
            }
            if (LowSulfurContent > HighSulfurContent)
            {
                ErrorMessage = "Low Sulfur content value cannot be more than High Sulfur content value.";
                return;
            }

            decimal time = 0.00m;
            decimal dt = CalculationStepHours;
            decimal fuel = FuelOilQty;
            decimal rate = TotalFuelConsumptionRate;

            decimal sulfurContent = HighSulfurContent;

            for (int i = 0; i <= 1000; i++)
            {

                time += dt;

                var sulfurFromServTank = LowSulfurContent * dt * rate * 10m;
                var sulfurInService = dt * rate * sulfurContent * 10m;

                decimal dS = (fuel == 0m)
                    ? (sulfurFromServTank - sulfurInService) / 10m
                    : (sulfurFromServTank - sulfurInService) / fuel / 10m;

                sulfurContent = sulfurContent + dS;


                if (time > 0.20m)
                {
                    if (sulfurContent <= SelectedValueForRequiredSulfurContent)
                    {
                        TimeRequired = Math.Round(time, 2, MidpointRounding.AwayFromZero).ToString("0.00");
                        return;
                    }
                }
            }

            TimeRequired = ">200";
        }
    }
}
