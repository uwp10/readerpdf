// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PDFPage.cs" company="vlady-mix">
//    Fabricio Altamirano  2016
//  </copyright>
//  <summary>
//    The definition of  PDFPage.cs
//  </summary>
//  --------------------------------------------------------------------------------------------------------------------
namespace PdfService
{
    using Windows.Data.Pdf;
    using Windows.UI.Xaml.Media.Imaging;

    public class PDFPage
    {
      public  PdfPage PdfPage { get; set; }
      public  BitmapImage ImagePreview { get; set; } 
    }
}