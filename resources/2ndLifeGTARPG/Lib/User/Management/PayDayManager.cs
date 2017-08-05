using System;
using System.Collections.Generic;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using Newtonsoft.Json.Linq;

namespace TerraTex_RL_RPG.Lib.User.Management
{
    public class PayDayManager : Script
    {
        [Command("payday", Group = "general", SensitiveInfo = false)]
        public void PayDayCommand(Client player)
        {

            Dictionary<string, double> income = (Dictionary<string, double>)player.getData("LastPayDayIncome");
            Dictionary<string, double> outgoings = (Dictionary<string, double>)player.getData("LastPayDayOutgoings");

            Dictionary<string, Dictionary<string, double>> payDay = new Dictionary<string, Dictionary<string, double>>();
            payDay.Add("Income", income);
            payDay.Add("Outgoings", outgoings);

            player.triggerEvent("openPayDayUI", JObject.FromObject(payDay).ToString());
        }

        public class Category
        {
            private readonly string _name;
            private readonly string _identifier;
            private readonly double _tax;
    
            public static readonly Category Tax =new Category("Tax", "Steuern", 0);
            public static readonly Category Job =new Category("Job", "Jobgehälter", 0.25);
            public static readonly Category BasicSalary =new Category("BasicSalary", "Grundgehalt", 0);

            private Category(String identifier, String name, double tax)
            {
                this._identifier = identifier;
                this._name = name;
                this._tax = tax;
            }

            public override String ToString()
            {
                return _name;
            }

            public String ToIdentifierString()
            {
                return _identifier;
            }

            public double GetTax()
            {
                return _tax;
            }

            public bool Equals(Category obj)
            {
                return obj.ToIdentifierString().Equals(_identifier);
            }
        }


        public static void AddOutgoingToPayDay(Client player, double amount, Category category)
        {
            Dictionary<string, double> outgoings = (Dictionary<string, double>)player.getData("PayDayOutgoings");

            if (amount < 0)
            {
                amount = -amount;
                TTRPG.Api.consoleOutput(LogCat.Warn, "Tried to add negative amount to income of payday in category '" + category.ToString() + "' . Used positive amount instead.");
            }

            if (outgoings.ContainsKey(category.ToString()))
            {
                outgoings.Set(category.ToString(), outgoings.Get(category.ToString()) + amount);
            }
            else
            {
                outgoings.Add(category.ToString(), amount);
            }
        }

        public static void AddIncomeToPayDay(Client player, double amount, Category category, bool withTax)
        {
            Dictionary<string, double> income = (Dictionary<string, double>)player.getData("PayDayIncome");

            if (amount < 0)
            {
                amount = -amount;
                TTRPG.Api.consoleOutput(LogCat.Warn, "Tried to add negative amount to income of payday in category '" + category.ToString() + "' . Used positive amount instead.");
            }

            if (income.ContainsKey(category.ToString()))
            {
                income.Set(category.ToString(), income.Get(category.ToString()) + amount);
            }
            else
            {
                income.Add(category.ToString(), amount);
            }

            if (withTax && category.GetTax() > 0)
            {
                double tax = category.GetTax() * amount;
                AddOutgoingToPayDay(player, tax, Category.Tax);
            }
        }

        public static void SendPayDay(Client player)
        {
            // Add additional 10 RP for each PayDay
            RpLevelManager.AddRpToPlayer(player, 10, false);

            Dictionary<string, double> income = (Dictionary<string, double>)player.getData("PayDayIncome");
            Dictionary<string, double> outgoings = (Dictionary<string, double>)player.getData("PayDayOutgoings");

            // default Values
            AddIncomeToPayDay(player, 250, Category.BasicSalary, false);

            double sum = 0;

            foreach (KeyValuePair<string, double> value in income)
            {
                sum += value.Value;
            }

            foreach (KeyValuePair<string, double> value in outgoings)
            {
                sum -= value.Value;
            }

            Dictionary<string, Dictionary<string, double>> payDay =
                new Dictionary<string, Dictionary<string, double>>();
            payDay.Add("Income", income);
            payDay.Add("Outgoings", outgoings);

            MoneyManager.ChangePlayerMoney(player, (float)sum, true, MoneyManager.Categorys.PayDay, "PayDay",
                JObject.FromObject(payDay).ToString());

            if (sum >= 0)
            {
                TTRPG.Api.sendNotificationToPlayer(player, "Zahltag! Dir wurden ~g~" + sum + " €~s~ überwiesen.");
            }
            else
            {
                TTRPG.Api.sendNotificationToPlayer(player, "Zahltag! Dir wurden ~r~" + sum + " €~s~ abgezogen.");
            }

            player.setData("LastPayDayIncome", new Dictionary<string, double>(income));
            player.setData("LastPayDayOutgoings", new Dictionary<string, double>(outgoings));
            income.Clear();
            outgoings.Clear();
            player.setData("PayDayIncome", income);
            player.setData("PayDayOutgoings", outgoings);
        }
    }
}