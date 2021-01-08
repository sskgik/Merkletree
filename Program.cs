//Please Check here
//generateProof() is described on line 321, and verifyProof() is described on line 460.
//Each method unit test is described in a separate sheet

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{

    public class MerkletreeProgram
    {
        //Please Entry Transaction hashes here(↓)
        private static string[] BeforeparseTxhash = new string[]
        {
          "d440c2e3e0a708ce82c1b8e01155f9f350b41b97020d754a77bc0621730ac2e5",
          "9842893448d6f7534f5f82737094804c4fe9ec551de0fc6844d945f4736db3e8",
          "fdacd62551099230552d776d1d92cff1a928172ea6e4970c976be939287e86ec",
          "af762cf30b5a536601f5159852de776d73942bf48e1d5adaf91937b3d8f773f3",
          "a4d26e4e35f0c821c9e71286fdcf4566f3bfc77dcd092684ab21ef34becb78f9",
          "828f1be2e3896a62c7a1ae1b5033a6f8540002d3b06b889181ec0e35612b3315",
          "0456df2b357f041d414211f10ed3e5b8dbf29141bd9de9c9594b97f843c9ae6c",
          "ca3789839b6e838343b089c6d206ccbfd5b1caa611453297b130cfb835e2dccb", 
          "9255a032a8b52f4ab23a349344a601f016b8997a3b8a533c8a64779f66b759ac" 
        };

        private static string[] GetTxhash = new string[0];

        private static string[] StorageTxhash = new string[0];

        private static string[] proof = new string[0];

        private static string Merklroot;

        /// <summary>
        /// Main method entry point
        /// </summary>
        /// <param name="args"></param>
        /// Process flow
        /// 1st: Calculation Merkletree's Array
        /// 2nd: Generate proof (All Transactionhashes)
        /// 3rd: Verifi Proof (previously) 
        /// * If you enter anything other than the created target transaction hash, it will be false.
        private static void Main(string[] args)
        {
            //Calculation MerkletreeHash&Merklroot start
            int i,j;
            Console.WriteLine("Transaction Hashes Convert to Hash(SHA-256)\n");
            (string[,] MerkletreeHash, string Merklroot, int num) = Merkletree(BeforeparseTxhash);
            if(MerkletreeHash[0,0]==Merklroot)
            {
                Console.WriteLine("This is the end\n");
                return;
            }

            //GenerateProof
            Console.WriteLine("\n");
            Console.WriteLine("Start Generate proof !");
            
            Console.WriteLine("\n");
            MerkletreeProgram test = new MerkletreeProgram();
            //Calcurate proof element
            var proofnum = GetPrimaryElement(BeforeparseTxhash.Length) + 2;
            string[,] proofes = new string[BeforeparseTxhash.Length, proofnum];
            for (i=0; i<=BeforeparseTxhash.Length-1; i++)
            {
                proof = test.GenerateProof(MerkletreeHash, BeforeparseTxhash[i]);  //Generate proof. (if inputTxHash does not exist in BeforeparseTxhash is not make proof)
                Console.WriteLine("\n");
                Console.WriteLine("Regist Infomation in proof!");
                for (j = 0; j <= proofnum - 1; j++)
                {
                    proofes[i, j] = proof[j];
                    if (proof[j] == null)
                    {
                        break;
                    }
                    Console.WriteLine("TxHash:\t{0}", proof[j]);
                }
            }
            

            //Verifiproof from proof & Transaction hash
            Console.WriteLine("\n");
            Console.WriteLine("Please Type Transaction hash you want to check");
            var checktransaction = Console.ReadLine();
            Console.WriteLine("\n");
            var Judge = test.VerifyProof(Merklroot, proofes, checktransaction);
            if (Judge == true)
            {
                Console.WriteLine("your Transaction Hash found in this Proof!\nblock Merkleroot:\t" + Merklroot + "\nYour input TxHash:\t" + checktransaction + "\nResult:\ttrue");
            }
            else
            {
                Console.WriteLine("your Transaction Hash is nothing in this Proof! \nblock Merkleroot;\t" + Merklroot + "\nYour input TxHash:\t" + checktransaction + "\nResult:\tfalse");
            }
        }

        /// <summary>
        /// Merkletree's Calcuration to Merkleroot and make merkletree table
        /// </summary>
        /// <param name="TxHash">Hash of the input calculation first target array</param>
        private static Tuple<string[,],string ,int> Merkletree(string[] InputTxHash)
        {
            int i,Element, PrimaryElement;
            int num = 0;
            var SecondaryElement = InputTxHash.Length; //get array Element in MerkletreeHash (Secondary element)
            //case: InputTxhash element = 1 
            if(SecondaryElement == 1)
            {
                PrimaryElement = -2;
            }
            //case: InputTxhash element = 2
            else if(SecondaryElement == 2)
            {
                PrimaryElement = 0;
            }
            //case: other
            else 
            {
                PrimaryElement = GetPrimaryElement(SecondaryElement);   //get array Element in MerkletreeHash (Primary element)
            }
             
            string[,] MerkletreeHash = new string[PrimaryElement + 3, SecondaryElement];   //MerkltreeTxhash Array declaration
            string[]  ForCalcHash = new string[SecondaryElement];                          //For Calcuration Array in this Method

            //bottom layer
            for (i = 0; i <= (SecondaryElement - 1); i++)
            {
                MerkletreeHash[num,i] = InputTxHash[i];
                Console.WriteLine("TxHash_{0}:\t{1}", i + 1, MerkletreeHash[num,i]);
            }
            num++;
            Console.WriteLine("\n");
            Console.WriteLine("{0} time:{1} input elements of hash", num, SecondaryElement);
            //Next layer
            for (i=0; i<= SecondaryElement - 1; i++)
            {
                if (SecondaryElement == 1)
                {
                    continue;
                }
                MerkletreeHash[num, i] = ConvertHash(InputTxHash[i]);
                ForCalcHash[i] = MerkletreeHash[num, i];
                Console.WriteLine("TxHash_{0}:\t{1}", i + 1, MerkletreeHash[num, i]);
            }
            num++;
            //From the next layer onwards
            Element = SecondaryElement;
            do
            {   //Case: InputTxHash element is 1
                if (Element == 1 && GetTxhash.Length == 0)
                {
                    Element = 2;
                    GetTxhash = InputTxHash;
                }
                ////Case: InputTxHash element is 2
                else if (Element == 2 && GetTxhash.Length == 0)
                {
                    Element = 2;
                    GetTxhash = ForCalcHash;
                }
                //Case: other (Less than)
                else if (GetTxhash.Length == 0)
                {
                    Console.WriteLine("\n");
                    GetTxhash = CalcTxHash(ForCalcHash, num);
                    Element = GetTxhash.Length;
                    for (i=0;i<=(Element - 1);i++)
                    {
                        MerkletreeHash[num, i] = GetTxhash[i];
                    }
                    num++;
                }
                else
                {
                    Console.WriteLine("\n");
                    StorageTxhash = GetTxhash;
                    GetTxhash = new string[0];
                    GetTxhash = CalcTxHash(StorageTxhash, num);
                    Element = GetTxhash.Length;
                    for (i = 0; i <= Element - 1; i++)
                    {
                        MerkletreeHash[num, i] = GetTxhash[i];
                    }
                    num++;
                }
            } while (Element != 2);

            Console.WriteLine("\n");
            //judge input element is 1 or other
            if (InputTxHash.Length == 1)
            {
                Console.WriteLine("TxHash element is 1");
                Merklroot = GetTxhash[0];
                MerkletreeHash[0, 0] = Merklroot;

            }
            else
            { 
                Merklroot = FinalCalcTxHash(GetTxhash,num);
                MerkletreeHash[num, 0] = Merklroot;
                num++;
            }
            Console.WriteLine("\nMerkleroot is:\t{0}", Merklroot);
            return new Tuple<string[,],string , int>(MerkletreeHash, Merklroot, num);  //return MerkleTree array & Calc Times
        }

        /// <summary>
        /// Calculation hash according to the mark tree
        /// </summary>
        /// <param name="TxHash">Hash of the input calculation target array</param>
        /// <param name="num">Number of calculations</param>
        /// <returns></returns>
        private static string[] CalcTxHash(string[] TxHash, int num)
        {
            
            int i, Max;
            int Element = TxHash.Length;   //get Element in array

            //Display the number of inputs and the number of calculations
            Console.WriteLine("{0} time:{1} input elements of hash",num, Element);
                    
            //Case: Odd number of array elements
            if (Element % 2 == 1)
            {
                string[] TotalTxhash = new string[(Element + 1) / 2];
                Max = (Element - 1) / 2;
                for (i = 0; i <= Max; i++)
                {
                    int j = 2 * i + 1;
                    if (i == Max)
                    {
                        //Concat hash of sha256
                        var sumhash = string.Concat(TxHash[j-1], TxHash[j - 1]);
                        TotalTxhash[i] = ConvertHash(sumhash);
                        Console.WriteLine("TxHash_{0}:\t{1}", i + 1, TotalTxhash[i]);
                    }
                    else
                    {
                        var sumhash = string.Concat(TxHash[j-1], TxHash[j]);
                        TotalTxhash[i] = ConvertHash(sumhash);
                        Console.WriteLine("TxHash_{0}:\t{1}", i + 1 ,TotalTxhash[i]);
                    }
                }
                return TotalTxhash;
            }
            //Case: Even number of array elements
            else
            {
                string[] TotalTxhash = new string[Element / 2];
                Max = Element / 2 -1;
                for (i = 0; i <= Max; i++)
                {                    
                   int j =  2 * i + 1;
 
                   var sumhash = string.Concat(TxHash[j-1], TxHash[j]);
                   TotalTxhash[i] = ConvertHash(sumhash);
                   Console.WriteLine("TxHash_{0}:\t{1}", i + 1, TotalTxhash[i]);
                }
                return TotalTxhash;
            }  
        }

        /// <summary>
        /// Required primary element calculation  (use Merkletree Method)
        /// </summary>
        /// <param name="num">Number of transaction Hashes</param>
        /// <returns>PrimaryElement</returns>
        private static int GetPrimaryElement(int num)
        {
            var Elementwithlog = Convert.ToDouble(num);
            var result1 = Math.Log(Elementwithlog, 2.0); //Calc merkltree layer element num
            var result2 = Math.Floor(result1);           //Decimal point truncation
            var PrimaryElement = Convert.ToInt32(result2);

            return PrimaryElement;
        }

        /// <summary>
        /// Final Calculation of Merkle Tree   (use Merkletree Method)
        /// </summary>
        /// <param name="TxHash">Hash of the input calculation target array</param>
        /// <param name="num">Current number of calculations</param>
        /// <returns>result(Merkleroot value)</returns>
        private static string FinalCalcTxHash(string[] TxHash,int num)
        {
            Console.WriteLine("{0} time:2 input elements of hash",num);
            var sumhash = string.Concat(TxHash[0], TxHash[1]);
            var result = ConvertHash(sumhash);
            Console.WriteLine("TxHash_1:\t{0}", result);
            return result;
        }

        /// <summary>
        /// Convert HexDigit to Hash in Bitcoin specification
        /// </summary>
        /// <param name="str">Target string</param>
        /// <returns>hashed(string of Hexdicimal)</returns>
        private static string ConvertHash(string txhash)
        {  
            using SHA256 sha256 = SHA256.Create();   // Call SHA256 Instance
            byte[] encoded = Encoding.UTF8.GetBytes(txhash);   //encord taget string
            byte[] hash = sha256.ComputeHash(sha256.ComputeHash(encoded));   //double-SHA256
            string hashed = string.Concat(hash.Select(b => $"{b:x2}"));
            return hashed;
        }

        /// <summary>
        /// Generate proof method
        /// </summary>
        /// <param name="MerkletreeHashlist">Merkletree's Array</param>
        /// <param name="targetTxHash">Generate proof method</param>
        /// <returns>Storageproof</returns>
        public string[] GenerateProof(string[,] MerkletreeHashlist,string targetTxHash)
        {
            int i,j;
            string RegistHash;

            int Primarymax = MerkletreeHashlist.GetLength(0);
            int Secondarymax = MerkletreeHashlist.GetLength(1);
            string[] Storageproof = new string[Primarymax-1]; //return array
            string[] Storagearray = new string[Secondarymax]; //For Calc Array

            for (i = 0; i <= Secondarymax - 1; i++)
            {
                Storagearray[i] = MerkletreeHashlist[0, i];
            }
            //Check input Transaction Hash exist or no exist.
            int check = Array.IndexOf(Storagearray, targetTxHash);
            if(check == -1)
            {
                Console.WriteLine("\n");
                Console.WriteLine("The element below does not exist in the array\nNot make proof\n\n{0}", targetTxHash);
                return Storageproof;
            }
            Storageproof[0] = targetTxHash;   //regist Taget TxHash of bottom layer 

            int max = Secondarymax;
            RegistHash = Judgeelement(Storagearray, check, max);    //Get RegistHash by MerkletreeHashlist above the bottom layer
            Storageproof[1] = RegistHash;    //regist Taget TxHash of layer above the bottom(Next layer)  
            string[] StorageTxhash = new string[Secondarymax];

            for (i = 1; i <= Primarymax - 1; i++)
            {
                for (j = 0; j <= Secondarymax - 1; j++)
                {
                    StorageTxhash[j] = MerkletreeHashlist[i, j];
                }
                if(StorageTxhash.Length == 2)
                {
                    break;
                }
                else if(StorageTxhash[2] == null || StorageTxhash[2] == "")   // StorageTxhash[2] == ""  for test element
                {
                    break;
                }
                else 
                {
                    var RegistParentNum = RegistTxHashNum(StorageTxhash, RegistHash);
                    //Conditions that do not occur by design! Test code error countermeasures
                    if (RegistParentNum == -1)
                    {
                        return Storageproof;
                    }
                    RegistHash = MerkletreeHashlist[i+1 , RegistParentNum];
                    if(RegistHash == null)
                    {
                        RegistHash = MerkletreeHashlist[i + 1, RegistParentNum-1];
                    }
                    Storageproof[i+1] = RegistHash;
                }  
            }
            return Storageproof;
        }

        /// <summary>
        /// Judgeelement at First
        /// </summary>
        /// <param name="TargetArray">Bottom layer's Array</param>
        /// <param name="indexnum">The Element number of Target transaction for creating proof</param>
        /// <param name="max">Bottom layer's Element max number</param>
        /// <returns>RegistHash (Next layer hash in proof)</returns>
        private static string Judgeelement(string[] TargetArray,int indexnum,int max)
        {
            string RegistHash;
            //Case; Even number of indexnum
            if ((indexnum % 2) == 0)
            {
                if (indexnum + 1 == max)
                {
                    RegistHash = ConvertHash(TargetArray[indexnum]);
                }
                else
                {
                    RegistHash = ConvertHash(TargetArray[indexnum + 1]);
                }
            }
            //case Odd number of indexnum
            else
            {
                    RegistHash = ConvertHash(BeforeparseTxhash[indexnum - 1]);
            }
            return RegistHash;
        }


        /// <summary>
        /// Get Parent array Target Element at above the Second layer 
        /// </summary>
        /// <param name="TxHash">Current array</param>
        /// <param name="InputHash">Saved in the Current layer's Hash</param>
        /// <returns>GetTagetElement</returns> //Parent layer's Regist Target element
        private static int RegistTxHashNum(string[] TxHash, string InputHash)
        {
            int GetTagetElement;
            int arraynum = Array.IndexOf(TxHash, InputHash);
            if(arraynum == -1)
            {
                GetTagetElement = -1;
            }
            else if (arraynum  == 0 || (arraynum / 4) % 2 == 0)   //Regist Taget element number of parent layer Gettin
            {
                if (arraynum % 4 == 0 || arraynum % 4 == 1)
                {
                    GetTagetElement = (arraynum / 2) + 1;
                }
                else
                {
                    GetTagetElement = (arraynum / 2) - 1;
                }
            }
            else
            {
                if (arraynum % 4 == 0 || arraynum % 4 == 1)
                {
                    GetTagetElement = (arraynum / 2) + 1;
                }
                else
                {
                    GetTagetElement = (arraynum / 2) - 1;
                }
            }
            return GetTagetElement;
        }

        /// <summary>
        /// Verify Proof method
        /// </summary>
        /// <param name="Merkleroot">Merkltree Top</param>
        /// <param name="proof">Merkle PATH</param>
        /// <param name="CheckTransaction">You want to Check Transaction Hash</param>
        /// <returns>result</returns>
        public bool VerifyProof(string Merkleroot, string[,] proofes, string CheckTransaction)
        {
            int i,j, checkexist;
            int num = 0;
            var verifilist = new List<string>();

            var secondaryelement = proofes.GetLength(1);
            string[] proof = new string[secondaryelement];
            //check the proofes element Whether it contains CheckTransaction
            for (i=0;i<=proofes.GetLength(0)-1;i++)
            {
                for(j=0;j<=secondaryelement - 1;j++)
                {
                    proof[j] = proofes[i, j]; 
                }
                checkexist = Array.IndexOf(proof, CheckTransaction);
                if (checkexist != -1)
                {
                    break;
                }
                //Check Transaction Hash if it exists in the created proofes
                else if (i == proofes.GetLength(0) - 1 && checkexist == -1)
                {
                    return false;
                }
             
            }
            //Calculate merklproof from proof & Verify
            for (i = 0; i <= proof.Length - 1; i++)
            {
                if (i == 0)
                {
                    var verifi = ConvertHash(proof[i]);
                    verifilist.Add(verifi);
                    continue;
                }
                for (j = 1; j <= Math.Pow(2, i-1); j++)
                {
                   (var verifi1, var verifi2) = OtherPatternCalc(proof[i], verifilist[num]);
                   verifilist.Add(verifi1);
                   verifilist.Add(verifi2);
                   num++;
                }
            }
            var check = verifilist.IndexOf(Merkleroot);
            if (check == -1)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Calcurate hash from lower layer
        /// </summary>
        /// <param name="str1">parameta 1(proof element)</param>
        /// <param name="str2">parameta 2(get method hash)</param>
        /// <returns>Tuple<string, string>(verifi1, verifi2)</returns>
        private static Tuple<string,string> OtherPatternCalc(string str1,string str2)
        {
            var sumhash1 = string.Concat(str1, str2);
            var sumhash2 = string.Concat(str2, str1);
            var verifi1 = ConvertHash(sumhash1);
            var verifi2 = ConvertHash(sumhash2);
            return new Tuple<string, string>(verifi1, verifi2);
        }
    }
}
