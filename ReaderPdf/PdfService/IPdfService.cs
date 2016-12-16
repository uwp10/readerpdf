// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPdfService.cs" company="vlady-mix">
//    Fabricio Altamirano  2016
//  </copyright>
//  <summary>
//    The definition of  IPdfService.cs
//  </summary>
//  --------------------------------------------------------------------------------------------------------------------
namespace PdfService
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Windows.Data.Pdf;
    using Windows.Storage;
    using Windows.UI.Xaml.Media.Imaging;

    /// <summary>
    ///     The PdfService interface.
    /// </summary>
    public interface IPdfService
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the resolution.
        /// </summary>
        uint Resolution { get; set; }

        /// <summary>
        ///     Gets or sets the total pages.
        /// </summary>
        uint TotalPages { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// The get page.
        /// </summary>
        /// <param name="numPage">
        /// The num page.
        /// </param>
        /// <returns>
        /// The <see cref="BitmapImage"/>.
        /// </returns>
        BitmapImage GetPageAsBitmapImage(int numPage);

        /// <summary>
        /// The get pages.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        List<BitmapImage> GetPagesAsBitmapImages();

        /// <summary>
        /// The get pdf page.
        /// </summary>
        /// <param name="numPage">
        /// The num page.
        /// </param>
        /// <returns>
        /// The <see cref="PdfPage"/>.
        /// </returns>
        PdfPage GetPdfPage(int numPage);

        /// <summary>
        /// The get pdf pages.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        List<PDFPage> GetPDFPages(); 

        /// <summary>
        /// The load from local file.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task LoadFromLocalFile();

        /// <summary>
        /// The load file.
        /// </summary>
        /// <param name="pdfFile">
        /// The pdf file.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task LoadFromStorageFile(IStorageFile pdfFile);


        Task LoadFromUrl(Uri uriLocation);

        /// <summary>
        /// The save page to jpg.
        /// </summary>
        /// <param name="numPage">
        /// The num page.
        /// </param>
        /// <param name="outputFile">
        /// The output file.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task SavePageToJpg(uint numPage, IStorageFile outputFile);

        #endregion
    }
}