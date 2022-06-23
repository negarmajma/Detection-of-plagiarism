using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using System.IO;

namespace AnalyzerViewer
{
    public partial class MainForm : Form
    {
        public BindingList<AnalyzerInfo> AnalyzerList = new BindingList<AnalyzerInfo>();
        public BindingList<AnalyzerView> AnalyzerViews = new BindingList<AnalyzerView>();

        public MainForm()
        {
            InitializeComponent();

            AnalyzerList.Add(new AnalyzerInfo("Keyword Analyzer", "\"Tokenizes\" the entire stream as a single token.",  new Lucene.Net.Analysis.KeywordAnalyzer()));
            AnalyzerList.Add(new AnalyzerInfo("Whitespace Analyzer", "An Analyzer that uses WhitespaceTokenizer.",  new WhitespaceAnalyzer()));
            AnalyzerList.Add(new AnalyzerInfo("Stop Analyzer", "Filters LetterTokenizer with LowerCaseFilter and StopFilter.",  new StopAnalyzer()));
            AnalyzerList.Add(new AnalyzerInfo("Simple Analyzer", "An Analyzer that filters LetterTokenizer with LowerCaseFilter.",  new Lucene.Net.Analysis.SimpleAnalyzer()));
            AnalyzerList.Add(new AnalyzerInfo("Standard Analyzer", "Filters StandardTokenizer with StandardFilter, LowerCaseFilter and StopFilter, using a list of English stop words.",  new StandardAnalyzer()));

            AnalyzerViews.Add(new AnalyzerViewer.TermAnalyzerView());
            AnalyzerViews.Add(new TermWithOffsetsView());
            AnalyzerViews.Add(new TermFrequencies());
            
            tbDescription.DataBindings.Add(new Binding("Text", AnalyzerList, "Description"));

            cbAnalysers.DisplayMember = "Name";
            cbAnalysers.ValueMember = "LuceneAnalyzer";
            cbAnalysers.DataSource = AnalyzerList;

            cbViews.DisplayMember = "Name";
            cbViews.DataSource = AnalyzerViews;

            cbAnalysers.SelectedIndex = 0;
            cbViews.SelectedIndex = 0;

            cbAnalysers.SelectedValueChanged += new EventHandler(cbAnalysers_SelectedValueChanged);
            cbViews.SelectedValueChanged += new EventHandler(cbViews_SelectedValueChanged);
            tbSourceText.TextChanged += new EventHandler(tbSourceText_TextChanged);

            tbSourceText.Text = "The quick brown fox jumped over the lazy dog.";
            AnalyzeText();
        }

        void cbViews_SelectedValueChanged(object sender, EventArgs e)
        {
            AnalyzeText();            
        }

        void tbSourceText_TextChanged(object sender, EventArgs e)
        {
            AnalyzeText();
        }

        void cbAnalysers_SelectedValueChanged(object sender, EventArgs e)
        {
            AnalyzeText();
        }

        public void AnalyzeText()
        {
            Analyzer analyzer = cbAnalysers.SelectedValue as Analyzer;

            int termCounter = 0;

            if (analyzer != null)
            {
                StringBuilder sb = new StringBuilder();

                AnalyzerView view = cbViews.SelectedValue as AnalyzerView;

                StringReader stringReader = new StringReader(tbSourceText.Text);

                TokenStream tokenStream = analyzer.TokenStream("defaultFieldName", stringReader);

                tbOutputText.Text =  view.GetView(tokenStream, out termCounter).Trim();
            }

            lblStats.Text = string.Format("Total of {0} Term(s) Found.", termCounter);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

    }
}
