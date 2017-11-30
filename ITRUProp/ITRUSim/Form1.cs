using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ITRUSim
{
    public partial class ITRUSimulator : Form
    {
        //Initialization of Variables
        int p = 0, m = 0, f = 0, g = 0, r = 0, q = 0, Fp = 0, Fq = 0, h = 0, checkFp = 0, checkFq = 0, ivnum = 0;
        int left = 0;
        int cont_f = 0, cont_g = 0, cont_r = 0;
        int past_f = 0;
        int noSuccess = 0, noFail = 0, noInverseFails = 0, noOtherFails = 0;
        bool encryptSuccess = false;
        bool isSingle = false;
        string msg = null, nmsg = null;
        byte[] utf8Bytes;
        byte[] utf8BytesFinal;

        List<int> pmsg = new List<int>();
        List<int> emsg = new List<int>();
        List<int> amsg = new List<int>();
        List<int> cmsg = new List<int>();
        List<int> iv = new List<int>();

        //Toggle for manual input of parameters vs autorun
        private void btnToggle_Click(object sender, EventArgs e)
        {
            if(btnToggle.Text == "Disable")
            {
                txtP.Enabled = false;
                txtQ.Enabled = false;
                txtR.Enabled = false;
                txtG.Enabled = false;
                txtF.Enabled = false;
                txtM.Enabled = false;
                btnToggle.Text = "Enable";
            }
            else
            {
                txtP.Enabled = true;
                txtQ.Enabled = true;
                txtR.Enabled = true;
                txtG.Enabled = true;
                txtF.Enabled = true;
                txtM.Enabled = true;
                btnToggle.Text = "Disable";
            }
        }

        public ITRUSimulator()
        {
            InitializeComponent();
        }

        //isPrime function - checks if a is prime
        public bool isPrime(int a)
        {
            int fact = 0;
            for (int i = 2; i < a / 2; i++)
            {
                if (a % i == 0)
                    fact++;
            }
            if (fact > 0)
                return false;
            else
                return true;
        }

        //Extended Euclidean Algorithm module
        public int extendedEuclidean(int a, int b)
        {
            int q = 0, r1 = b, r2 = a;
            int r = r1 - (q * r2);
            int t1 = 0, t2 = 1, t = 0;
            if (isSingle)
                txtCompute.Text += "q = 0\nr1 = "+b+"\nr2 = "+a+"\n r = r1 - (q*r2)\nt1 = 0\nt2 = 1\nt = 0\n\n";
            while (r != 0)
            {
                q = r1 / r2;
                r = r1 - (q * r2);
                t = t1 - (q * t2);
                if (isSingle)
                {
                    txtCompute.Text += "q = r1/r2 = "+r1+"/"+r2+" = "+q+"\n";
                    txtCompute.Text += "r = r1 - (q*r2) = "+r1+" - ("+q+"*"+r2+") = " + r + "\n";
                    txtCompute.Text += "t = t1 - (q*t2) = " + t1 + " - (" + q + "*" + t2 + ") = "+t+"\n";
                }
                    
                r1 = r2;
                if (isSingle)
                    txtCompute.Text += "r1 = r2 = " + r2 + "\n";
                r2 = r;
                if (isSingle)
                    txtCompute.Text += "r2 = r = " + r + "\n";
                t1 = t2;
                if (isSingle)
                    txtCompute.Text += "t1 = t2 = " + t2 + "\n";
                t2 = t;
                if (isSingle)
                    txtCompute.Text += "t2 = t = " + t + "\n\n";
            }
            int d = 0;
            if (t1 < 0)
            {
                d = t1 + b;
            }
            else
            {
                d = t1;
            }
            return d;
        }

        //Clears all fields and instantiates all variables to default value
        public void clearAll()
        {
            txtP.Clear();
            txtQ.Clear();
            txtR.Clear();
            txtF.Clear();
            txtG.Clear();
            txtM.Clear();
            txtContR.Text = "100";
            txtContF.Text = "100";
            txtContG.Text = "100";
            txtCount.Text = "10";
            txtCycle.Text = "1";
            txtInitMsg.Clear();
            txtASCII.Clear();
            txtKpb.Clear();
            txtKpq.Clear();
            txtEncrypted.Clear();
            txtDecrypt1.Clear();
            txtDecrypt2.Clear();
            txtFinalMsg.Clear();
            txtErrors.Clear();
            txtChkFp.Clear();
            txtChkFq.Clear();
            txtCompute.Clear();
            dataStats.Rows.Clear();
            p = 0;
            m = 0;
            f = 0;
            g = 0;
            r = 0;
            q = 0;
            Fp = 0;
            Fq = 0;
            h = 0;
            msg = null;
            nmsg = null;
            encryptSuccess = false;
            pmsg = new List<int>();
            emsg = new List<int>();
            amsg = new List<int>();
            cmsg = new List<int>();

        }

        //Clears fields for Encryption
        public void clearEncrypt()
        {
            txtASCII.Clear();
            txtKpb.Clear();
            txtKpq.Clear();
            txtEncrypted.Clear();
            txtDecrypt1.Clear();
            txtDecrypt2.Clear();
            txtFinalMsg.Clear();
            txtErrors.Clear();
            txtChkFp.Clear();
            txtChkFq.Clear();
            txtCompute.Clear();
            Fp = 0;
            Fq = 0;
            h = 0;
            encryptSuccess = false;
            msg = null;
            nmsg = null;
            pmsg = new List<int>();
            emsg = new List<int>();
            amsg = new List<int>();
            cmsg = new List<int>();
        }
        //Clears fields for Decryption
        public void clearDecrypt()
        {
            txtDecrypt1.Clear();
            txtDecrypt2.Clear();
            txtFinalMsg.Clear();
            amsg = new List<int>();
            cmsg = new List<int>();
        }
        //Encryption Module
        public void Encrypt()
        {
            if (isSingle)
                txtCompute.Text += "ITRU SIMULATION: \n\n";
            //Instantiate m' = 0
            m = 0;
            //Toggling initalization through textbox vs auto initalization
            if (btnToggle.Text == "Disable")
            {
                if (isSingle)
                    txtCompute.Text += "Randomization is disabled. Paraterers are manual.";
                //Initialization through textbox
                p = (int)Decimal.Parse(txtP.Text);
                q = (int)Decimal.Parse(txtQ.Text);
                r = (int)Decimal.Parse(txtR.Text);
                g = (int)Decimal.Parse(txtG.Text);
                f = (int)Decimal.Parse(txtF.Text);
                m = (int)Decimal.Parse(txtM.Text);      
                msg = txtInitMsg.Text;
                utf8Bytes = Encoding.UTF8.GetBytes(msg);
                //Converts msg to ASCII 8-bit decimal value
                for (int i = 0; i < utf8Bytes.Count(); i++)
                {
                    try
                    {
                        pmsg.Add(utf8Bytes[i]);
                        if (isSingle)
                            txtASCII.Text += "(" + pmsg[i] + ")";
                        encryptSuccess = true;
                    }
                    catch (Exception e)
                    {
                        //If msg character exceeds 8 bits
                        txtErrors.Text += "Error Converting Char: " + msg[i] + ". Not an ASCII 8-bit Character. \n";
                        encryptSuccess = false;
                    }
                }
            }
            else
            {
                //Initialization through autorun
                //For limits of random values
                cont_f = (int)Decimal.Parse(txtContF.Text);
                cont_r = (int)Decimal.Parse(txtContR.Text);
                cont_g = (int)Decimal.Parse(txtContG.Text);
                Random rand = new Random();
                //Select a random prime integer and assign to f'
                while(f%2==0 || f == past_f)
                {
                    f = rand.Next()%cont_f;
                }
                past_f = f;
                //Select two random integers and assign to r' and g'
                r = rand.Next()%cont_r;
                g = rand.Next()%cont_g;
                //Sets values to parameter textboxes
                txtP.Text = p.ToString();
                txtR.Text = r.ToString();
                txtG.Text = g.ToString();
                txtF.Text = f.ToString();
                msg = txtInitMsg.Text;
                utf8Bytes = Encoding.UTF8.GetBytes(msg);
                //Convert msg to ASCII 8-bit decimal
                for (int i = 0; i < utf8Bytes.Count(); i++)
                {
                    try
                    {
                        pmsg.Add(utf8Bytes[i]);
                        //Gets the value representing m' for (p' x r' x g' + f' x m)
                        if (m < pmsg[i])
                            m = pmsg[i];
                        if(isSingle)
                            txtASCII.Text += "(" + pmsg[i] + ")";
                        encryptSuccess = true;
                    }
                    catch (Exception e)
                    {
                        //If msg character exceeds 8 bits
                        txtErrors.Text += "Error Converting Char: " + msg[i] + ". Not an ASCII 8-bit Character. \n";
                        encryptSuccess = false;
                    }
                }
                m++;
                //Initialize p = m > randomPrime > 1000;
                p = rand.Next(m, 1000);
                while(!isPrime(p))
                {
                    p = rand.Next(m, 1000);
                }
                //Create IV Key
                ivnum = rand.Next(1,utf8Bytes.Count());
                for(int i = 0; i < ivnum; i++)
                {
                    iv.Add(rand.Next(1, 255));
                }
                txtM.Text = m.ToString();
                //Get the value of (p' x 'r x 'g + f' x m')
                int q_temp = p * r * g + f * m;
                q = q_temp++;
                //Find a prime number greater than q_temp and assign to q
                while (!isPrime(q))
                    q++;
                txtQ.Text = q.ToString();
            }
            //If msg has all ASCII characters
            if(encryptSuccess)
            {
                //Compute for f^-1 mod p and f^-1 mod q using Extended Euclidean
                if(isSingle)
                    txtCompute.Text += "Fp = extendedEuclidean(f,p)\n";
                Fp = extendedEuclidean(f, p);
                checkFp = (f * Fp) % p;
                if (isSingle)
                    txtCompute.Text += "Check: f*Fp mod p = " + f + "*" + Fp + " mod " + p + " = " + checkFp + "\n\n";
                txtChkFp.Text = checkFp.ToString();
                if (isSingle)
                    txtCompute.Text += "Fq = extendedEuclidean(f,q)\n";
                Fq = extendedEuclidean(f, q);
                checkFq = (f * Fq) % q;
                if (isSingle)
                    txtCompute.Text += "Check: f*Fq mod q = " + f + "*" + Fq + " mod " + q + " = " + checkFq + "\n\n";
                txtChkFq.Text = checkFq.ToString();
                //Display public key pair Kpb = (f', Fp)
                txtKpb.Text = "(" + f + ", " + Fp + ")";
                if (isSingle)
                    txtCompute.Text += "Public Key: Kpb = (" + f + ", " + Fp + ")\n\n";
                //Assign values to long to compensate for C# int limitations
                long pLong = p;
                long FqLong = Fq;
                long gLong = g;
                long qLong = q;
                //Compute private key Kph = (p' x Fq' x g') mod q'
                long hLong = (pLong * FqLong * gLong) % qLong;
                if (isSingle)
                    txtCompute.Text += "Private Key: Kph = (p x Fq x g) mod q = ("+pLong+" x "+FqLong+" x "+gLong+") mod "+qLong+" = "+hLong;
                int h = (int)hLong;
                txtKpq.Text = h.ToString();
                //Encrypt each decimal of msg using e' = ((r' x h)' + m') mod q'
                for (int i = 0; i < utf8Bytes.Count(); i++)
                {
                    emsg.Add(((r * h) + pmsg[i]) % q);
                }
                //Encrypt using CBC
                int j = 0;
                List<int> temp = new List<int>(), ivcopy = new List<int>();
                ivcopy.AddRange(iv);
                for (int i = 0; i < utf8Bytes.Count(); i++)
                {
                    emsg[i] = ivcopy[j] ^ emsg[i];
                    if (isSingle)
                        txtEncrypted.Text += "(" + emsg[i] + ")";
                    temp.Add(emsg[i]);
                    j++;
                    if (j == ivnum)
                    {
                        ivcopy.Clear();
                        ivcopy.AddRange(temp);
                        j = 0;
                    }
                }
            }
        }
        //Decryption Module
        public void Decrypt()
        {
            //Decrypt using CBC
            int j = 0;
            List<int> temp = new List<int>(), ivcopy = new List<int>();
            ivcopy.AddRange(iv);
            for (int i = 0; i < utf8Bytes.Count(); i++)
            {
                temp.Add(emsg[i]);
                emsg[i] = ivcopy[j] ^ emsg[i];
                j++;
                if (j == ivnum)
                {
                    ivcopy.Clear();
                    ivcopy.AddRange(temp);
                    j = 0;
                }
            }
            utf8BytesFinal = utf8Bytes;
            //Compute a' = (f' x e') mod q'
            for (int i = 0; i < utf8Bytes.Count(); i++)
            {
                amsg.Add((f * emsg[i]) % q);
                if(isSingle)
                    txtDecrypt1.Text += "(" + amsg[i] + ")";
            }
            //Get msg using C' = (Fp' x a') mod p'
            for (int i = 0; i < utf8Bytes.Count(); i++)
            {
                cmsg.Add((Fp * amsg[i]) % p);
                if (isSingle)
                    txtDecrypt2.Text += "(" + cmsg[i] + ")";
                utf8BytesFinal[i] = (byte)cmsg[i];
            }
            //Convert cmsg from ASCII decimal to ASCII character
            nmsg = Encoding.UTF8.GetString(utf8BytesFinal);
            txtFinalMsg.Text = nmsg;
            //Checks if msg = cmsg, updates status
            if (txtFinalMsg.Text.Equals(txtInitMsg.Text))
            {
                lblStatus.ForeColor = Color.ForestGreen;
                lblStatus.Text = "Success";
                noSuccess++;
            }
            else
            {
                lblStatus.ForeColor = Color.Firebrick;
                lblStatus.Text = "Failed";
                noFail++;
                if (checkFp == 1 && checkFq == 1)
                    noOtherFails++;
                else
                    noInverseFails++;
            }
            //Add process data to summary table
            dataStats.Rows.Add(msg, nmsg, lblStatus.Text, txtKpb.Text, txtKpq.Text);
            
        }

        private void btnEncryptDecrypt_Click(object sender, EventArgs e)
        {
            Stopwatch sw = Stopwatch.StartNew();
            isSingle = true;
            clearEncrypt();
            Encrypt();
            if (encryptSuccess)
                Decrypt();
            sw.Stop();
            txtTime.Text = (sw.ElapsedMilliseconds/1000.00).ToString()+"s";
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            isSingle = true;
            clearEncrypt();
            Encrypt();
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            if(encryptSuccess)
            {
                clearDecrypt();
                Decrypt();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            clearAll();
        }


        private void btnCycle_Click(object sender, EventArgs e)
        {
            isSingle = false;
            dataCount.Rows.Clear();
            pgLoad.Value = 0;
            float totalPercent = 0;
            int count = 0;
            int cycle = int.Parse(txtCycle.Text.ToString());
            int totNoSuccess = 0, totNoFail = 0, totInverseFail = 0, totOtherFail = 0;
            for(int j=0; j<cycle; j++)
            {
                noSuccess = 0;
                noFail = 0;
                noInverseFails = 0;
                noOtherFails = 0;
                dataStats.Rows.Clear();
                count = int.Parse(txtCount.Text);
                pgLoad.Maximum = count*cycle;
                pgLoad.Step = 1;
                for (int i = 0; i < count; i++)
                {
                    clearEncrypt();
                    Encrypt();
                    if (encryptSuccess)
                        Decrypt();
                    pgLoad.PerformStep();
                }
                float percentRate = ((float)noSuccess / (float)count) * 100;
                totNoSuccess += noSuccess;
                totNoFail += noFail;
                totInverseFail += noInverseFails;
                totOtherFail += noOtherFails;
                totalPercent += percentRate;
                dataCount.Rows.Add(noSuccess, noFail, percentRate+"%");
            }
            float finalPercent = totalPercent / (float)cycle;
            txtNoSuccess.Text = totNoSuccess.ToString();
            txtNoFail.Text = totNoFail.ToString();
            txtInverseFail.Text = totInverseFail.ToString();
            txtOtherFail.Text = totOtherFail.ToString();
            txtTotPercent.Text = finalPercent.ToString() + "%";
        }
    }
}
