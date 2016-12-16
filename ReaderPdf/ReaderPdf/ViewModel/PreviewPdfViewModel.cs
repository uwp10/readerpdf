// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreviewPdfViewModel.cs" company="vlady-mix">
//    Fabricio Altamirano  2016
//  </copyright>
//  <summary>
//    The definition of  PreviewPdfViewModel.cs
//  </summary>
//  --------------------------------------------------------------------------------------------------------------------

namespace ReaderPdf.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Windows.Storage.Pickers;
    using Commands;
    using PdfService;

    public class PreviewPdfViewModel : INotifyPropertyChanged
    {
        #region Private Fields

        private readonly PdfService pdfService;
        private List<PDFPage> pages;

        #endregion

        #region Constructors

        public PreviewPdfViewModel()
        {
            this.OpenFileCommand = new RelayCommand(this.OnOpenFile);
            this.pdfService = new PdfService();
        }

        #endregion

        #region Public Properties

        public RelayCommand OpenFileCommand { get; }

        public List<PDFPage> Pages
        {
            get { return this.pages; }
            set
            {
                this.pages = value;
                this.OnPropertyChanged(nameof(this.Pages));
            }
        }

        #endregion

        #region Protected Methods

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Private Methods

        private async void OnOpenFile()
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".pdf");
            var file = await picker.PickSingleFileAsync();
            if(file != null)
            {
                await this.pdfService.LoadFromStorageFile(file);
                this.Pages = this.pdfService.GetPDFPages();
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
    }
}