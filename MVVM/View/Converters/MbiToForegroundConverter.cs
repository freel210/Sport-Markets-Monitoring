using Model.Analisys;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using Model;
using CommonLibrary;
using System.Collections.ObjectModel;

namespace View.Converters
{
    public enum Gaps : byte { empty, none, one, more }

    [ValueConversion(typeof(ObservableCollection<MarketBookInformation>), typeof(SolidColorBrush))]
    public sealed class MbiToForegroundConverter : BaseConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mbi = value as ObservableCollection<MarketBookInformation>;
            if(mbi == null) return null;

            SolidColorBrush BlueColor = Brushes.Blue;
            SolidColorBrush BlackColor = Brushes.Black;
            SolidColorBrush PurpleColor = Brushes.Purple;

            var a = mbi.Last().CasesAnalisys;

            Gaps gaps = Gaps.empty;

            if(a[TradeCases.BL1B2].Gap == 0 && a[TradeCases.BL2B1].Gap == 0)
                gaps = Gaps.none;

            if(gaps != Gaps.none && a[TradeCases.BL1B2].Gap < 2 && a[TradeCases.BL2B1].Gap < 2)
                gaps = Gaps.one;

            if(gaps == Gaps.empty)
                gaps = Gaps.more;

            switch(gaps)
            {
                case Gaps.one:  return BlueColor;
                case Gaps.more: return PurpleColor;
                default:        return BlackColor;
            }
        }
    }
}
