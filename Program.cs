using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

namespace HW5
{
    class Program
    {
        static DateTime thisDate = DateTime.Now;
        static void Main(string[] args)
        {
            Decode();
        }
        private static void Decode()
        {
            long personalCode = 0;
            bool isCodeInt = false;
            while (!isCodeInt)
            {
                try
                {
                    Console.WriteLine("Kirjuta oma isikukoodi:");
                    personalCode = long.Parse(Console.ReadLine());
                    if (personalCode.ToString().Length == 11)
                        isCodeInt = true;
                    else
                        Console.WriteLine("Isikukood peab olema 11 numbritega");
                }
                catch (FormatException e)
                {
                    Console.WriteLine("Teie isikukood peab olema numbritest ja ilma spetsiaalsete märkideta", e);
                }
            }
            try
            {
                Console.Clear();
                string message = Constructor(personalCode);
                Console.WriteLine(message);
            }
            catch (Exception e)
            {
                Console.Clear();
                Console.WriteLine(e);
                Decode();
            }
        }

        private static string Constructor(long personalCode)
        {
            int year;
            string birthLocation;
            string decodedInfo = "";
            string code = personalCode.ToString();
            int birthNumber = int.Parse(code.Substring(9, 1));
            int checkNumber = int.Parse(code.Substring(10, 1));
            string sex = code.Substring(0, 1);
            string birthDate = code.Substring(1, 6);
            string serialNumber = code.Substring(7, 3);
            string sexDefined = DefineSexAndCentury(sex);
            year = int.Parse(sexDefined.Substring(0, 2));
            sex = sexDefined.Substring(2, sexDefined.Length - 2);
            DateTime date = ReturnBirthDate(birthDate, year);
            birthLocation = DecodeSerialNumber(serialNumber);
            CheckControllNum(code);
            decodedInfo = $"Decoding id code {personalCode}\n" +
                $"Birthdate is: {date.ToString("MM/dd/yyyy")}\n" +
                $"Sex: {sex}\n" +
                $"Place of birth: {birthLocation}\n" +
                $"Birth nr: {birthNumber}\n" +
                $"CheckNr: {checkNumber}";
            return decodedInfo;
        }

        private static string DefineSexAndCentury(string sex)
        {
            int num = int.Parse(sex);
            if (num == 1)
                return "18Mees";
            else if (num == 2)
                return "18Naine";
            else if (num == 3)
                return "19Mees";
            else if (num == 4)
                return "19Naine";
            else if (num == 5)
                return "20Mees";
            else if (num == 6)
                return "20Naine";
            else
                throw new Exception("Isikukoodi omanik ei ole veel sündinud");
        }

        private static DateTime ReturnBirthDate(string birthDate, int year)
        {
            string yearStr = year.ToString();
            DateTime finalDate = new DateTime();
            int month = int.Parse(birthDate.Substring(2, 2));
            int day = int.Parse(birthDate.Substring(4, 2));
            yearStr += int.Parse(birthDate.Substring(0, 2));
            year = int.Parse(yearStr);
            finalDate = new DateTime(year, month, day);
            //if (day)
            bool check = CheckYear(year);
            CheckMonth(month);
            GetDay(day, month, check);
            return finalDate;
        }

        private static bool CheckYear(int year)
        {
            bool check = false;
            if (year % 4 == 0)
                check = true;
            else
                check = false;
            return check;
        }

        private static void CheckMonth(int month)
        {
            if (month > 12 || month == 0)
                throw new Exception("Isikukoodis on pandud vale kuu");
        }

        private static int GetDay(int day, int month, bool check)
        {
            int days = 0;
            if (month == 7)
                days = 31;
            else if (month == 2)
                days = check ? 29 : 28;
            else if (month % 7 % 2 == 0)
                days = 30;
            else if (month % 7 % 2 == 1)
                days = 31;
            if (day > days || day == 0)
                throw new Exception("Isikukoodis on vale päev");
            return day;
        }

        private static string DecodeSerialNumber(string serialNumber)
        {
            int[] numberArray = new int[13]
            {
                10, 20, 220, 270, 370, 420, 470, 490, 520, 570, 600, 650, 700
            };
            string[] locationArray = new string[13]
            {
                "Kuressaare Haigla",
                "Tartu Ülikooli Naistekliinik",
                "Ida-Tallinna Keskhaigla või Pelgulinna sünnitusmaja",
                "Ida-Viru Keskhaigla",
                "Maarjamõisa Kliinikum",
                "Narva Haigla",
                "Pärnu Haigla",
                "Pelgulinna Sünnitusmaja või Haapsalu haigla",
                "Järvamaa Haigla ",
                "Rakvere",
                "Valga Haigla",
                "Viljandi Haigla",
                "Lõuna-Eesti Haigla"
            };
            int code = int.Parse(serialNumber);
            if (code == 0)
                throw new Exception("Teie järjekorra number isikukoodis ei saa olla 000 ");
            else if (code == 20)
                throw new Exception("Teie järjekorra number isikukoodis ei saa olla 020");

            string location = "";
            try
            {
                for (int i = 0; i < 13; i++)
                {
                    if (code < numberArray[i])
                    {
                        location = locationArray[i];
                        break;
                    }
                    else
                        continue;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return location;
        }

        private static void CheckControllNum(string code)
        {
            int codeSum = 0;
            int num = 0;
            int control = 0;
            for (int i = 0; i < 10; i++)
            {
                int newNum = int.Parse(code.Substring(i, 1));
                if (i == 9)
                {
                    num = newNum;
                    codeSum += num;
                }
                else
                {
                    num = newNum * (i + 1);
                    codeSum += num;
                }
            }
            control = codeSum % 11;
            if (control == 0)
            {
                for (int x = 3; x < 13; x++)
                {
                    for (int y = 0; y < 11; y++)
                    {
                        int newNum = int.Parse(code.Substring(y, 1));
                        if (x == 10)
                            num = newNum * 1;
                        else if (x == 11)
                            num = newNum * 2;
                        else if (x == 12)
                            num = newNum * 3;
                        num = newNum * x;
                    }
                }
            }
            if (control != int.Parse(code.Substring(10, 1)))
                throw new Exception("Isikukoodis on vale kontrolli number");
        }
    }
}
