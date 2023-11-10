using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hashFunction
{
    internal class hash
    {

        public hash()
        {

        }

        static private string FlowAlignment(string str)
        {
            //Подсчёт количества символов, которые мы должны добавить, если длинна в битах % 512 = 448,
            //то добавляем 448 бит, иначе добавляем те, которых не хватает до 448
            int a = System.Text.ASCIIEncoding.Unicode.GetByteCount(str) * 8 % 512 == 448 ? 
                0 : System.Text.ASCIIEncoding.Unicode.GetByteCount(str) * 8 % 512;

            //Строка отвечающая за битовое представление символа
            string result = str, addedBit = "1";

            for (int i = 0; i < 448 - a; i++)
            {
                addedBit += '0';

                if (addedBit.Length == 16)
                {
                    //С помощью Convert.ToInt32(addedBit, 2) получаем число представленное в 2-й сс
                    //затем используя Convert.ToString делаем строкой и удаляем первые 2 символа Substring(2)
                    //потом используем метод Int32.Parse для обработки полученного строкового значения
                    //и указываем что у нас строка в 16-й сс System.Globalization.NumberStyles.HexNumber
                    //и в итоге приводим к типу char
                    if (addedBit.Contains('1'))
                        result += (char)Int32.Parse(Convert.ToString(Convert.ToInt32(addedBit, 2)),
                        System.Globalization.NumberStyles.HexNumber);
                    else
                        result += (char)Convert.ToInt32(Convert.ToString(Convert.ToInt32(addedBit, 2)));
                    
                    addedBit = "";
                }
            }

            return result;
        }

        static private string AddedLenght(string massenge)
        {
            long lenMassenge = massenge.Length;
            //Младшие и старшие биты используются с маской FFFFFFFF, младшие считаются от 0,
            //старшие от 32
            int juniorBit = (int)((lenMassenge >> 0) & (long.MaxValue >> 32)), 
                seniorBit = (int)((lenMassenge >> 32) & (long.MaxValue >> 32));

            if (juniorBit >= 10 && seniorBit >= 10)
                return massenge + $"{juniorBit}{seniorBit}";

            else if (juniorBit < 10 && seniorBit >= 10)
                return massenge + $"{juniorBit}0{seniorBit}";

            else if (juniorBit >= 10 && seniorBit < 10)
                return massenge + $"{juniorBit}{seniorBit}0";

            else
                return massenge + $"{juniorBit}0{seniorBit}0";
        }

        static private string HashInLoop(string massenge, int A, int B, int C, int D)
        {
            string result = "";

            int tmpA = A, tmpB = B, tmpC = C, tmpD = D;
            //Цикл для раундов
            for (int i = 0; i < 4; i++)
            {
                //Цикл для блоков по 32 бита
                for (int j = 0; j < 16; j++)
                {
                    //Раундовая функция, расчитывается в каждом раунде по разному
                    int RF = 0;

                    //Расчёт раундовой функции в зависимости от раунда
                    switch (i)
                    {
                        case 0:
                            RF = (B & C) | (~B & D);
                            break;
                        case 1:
                            RF = (B & C) | (~D & C);
                            break;
                        case 2:
                            RF = B ^ C ^ D;
                            break;
                        case 3:
                            RF = C ^ (~D | B);
                            break;
                    }
                      
                    //Все действия до смены значения переменных выполняются по модулю 2^32
                    //и являются постоянными (прописаны в алгоритме)
                    A = (A + RF) % int.MaxValue;

                    string tmp = Convert.ToString((massenge.Length >> 32 * j) &
                        (int.MaxValue >> 32), 10);

                    //Для переворачивании строки
                    StringBuilder sb = new StringBuilder(tmp.Length);

                    for (int l = tmp.Length; l-- != 0;)
                        sb.Append(tmp[l]); 

                    tmp = sb.ToString();
                    //

                    A = (A + Convert.ToInt32(tmp)) % int.MaxValue;

                    double k = int.MaxValue * Math.Abs(Math.Sin(i + 16 * (j - 1)));

                    A = (A + (int)k) % int.MaxValue;

                    A = (A + B) % int.MaxValue;

                    //Меняем значения переменных 
                    C = B; B = A; A = D; D = C;
                }

                result += $"{A}{B}{C}{D}";
            }

            return result;
        }

        static public string Decoding(string massenge)
        {
            string tmpString = massenge, result = "";

            //Выравнивание потока
            result = FlowAlignment(tmpString);
            
            //Добавление длины сообщения
            result = AddedLenght(result);
            
            //Инициализирующий вектор
            int A = Convert.ToInt32("67452301", 16),
                B = Convert.ToInt32("EFCDAB89", 16),
                C = Convert.ToInt32("98BADCFE", 16),
                D = Convert.ToInt32("10325476", 16);

            //countBlock - количество блоков по 512 бит, processedBlocks - количество обработанных блоков
            int countBlock = result.Length * 16 / 512, processedBlocks = 0;

            string Block = "", tmpResult = result;

            result = "";
            //Цикл для обработки всех блоков по 512 бит сообщения 
            do
            {
                Block = tmpResult.Substring(32 * processedBlocks, 32);
                result += HashInLoop(Block, A, B, C, D);
                processedBlocks++;
            }
            while (processedBlocks != countBlock);

            return result;
        }
    }
}
