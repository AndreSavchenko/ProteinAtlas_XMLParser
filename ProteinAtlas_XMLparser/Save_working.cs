﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ProteinAtlas_XMLparser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

         private void button2_Click(object sender, EventArgs e)
        {
            parser(true, textBox1.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            parser(false, textBox1.Text);
        }

// Implementation

        private void parser(bool shortRun, string tissueType)
        {
            string XMLpth;string outputpth;
            openFileDialog1.FileName = "proteinatlas.xml";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                XMLpth = openFileDialog1.FileName;
                string validname=tissueType.Replace(@"\","_"); 
                outputpth=  Path.GetDirectoryName(XMLpth)+@"\"+validname+".txt";
                label2.Text= "Output:" + outputpth;

                if (label2.Width>276) this.Width = label2.Width + 30;

                int i = 0; int j=0;
                string ensg = "";
                string abId = "";
                string tissue = "";
                string tissueDescr = "";
                string iurl = "";
                string pid = "";
                int ensn = 0;
                {
                    StreamWriter sw = new StreamWriter(outputpth);
                    XmlTextReader reader = new XmlTextReader(XMLpth);
                    while (reader.Read() && j < 500000) // j wil be incremenrted in short run only
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (reader.Name == "identifier")        //doing nothing for the moment, just counting ENSG's
                                {
                                    ensg = reader.GetAttribute("id");
                                    //sw.WriteLine(ensg); 
                                    ensn++;
                                }

                                if (reader.Name == "antibody")          //doing nothing for the moment, could be used to create dictionaries.
                                {
                                    abId = reader.GetAttribute("id");
                                    //  sw.WriteLine(ensg + " " + abId);    
                                }

                                if (reader.Name == "tissue")            //doing nothing for the moment
                                {
                                    tissue = reader.ReadElementContentAsString();
                                    // sw.WriteLine(ensg + " " + abId+" "+tissue);
                                }

                                if ((reader.Name == "patient") && (tissue == tissueType))
                                {
                                    reader.ReadToFollowing("patientId");
                                    pid = reader.ReadElementContentAsString();

                                    reader.ReadToFollowing("snomed");
                                    tissueDescr = reader.GetAttribute("tissueDescription");
                                    tissueDescr += ", ";
                                    reader.ReadToFollowing("snomed");
                                    tissueDescr += reader.GetAttribute("tissueDescription");
                                    reader.ReadToFollowing("imageUrl");
                                    iurl = reader.ReadElementContentAsString();
                                    
                                    sw.WriteLine(ensg + ";" + abId + ";" + tissue + ";" + tissueDescr + ";" + pid + ";" + iurl);
                                    if (reader.ReadToNextSibling("imageUrl"))
                                    {
                                //        reader.ReadToFollowing("imageUrl");
                                        iurl = reader.ReadElementContentAsString();
                                        sw.WriteLine(ensg + ";" + abId + ";" + tissue + ";" + tissueDescr + ";" + pid + ";" + iurl);
                                    }


                                }
                                break;
                        }
                        i++;
                        if (i % 10000 == 0) { label1.Text = (i/1000).ToString() + "k lines analysed " + ensn.ToString() + " ENSG's found"; Application.DoEvents(); }
                        if (shortRun) j++;

                    }

                    sw.Close();
                    if (shortRun) System.Diagnostics.Process.Start(outputpth);

                }
            }

        }
    
       

        
    }



}
