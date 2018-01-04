using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace Wpf
{
    public class DatePickerCalendar
    {
        public static bool GetIsYear(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsYearProperty);
        }

        public static void SetIsYear(DependencyObject obj, bool value)
        {
            obj.SetValue(IsYearProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsYear.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsYearProperty =
            DependencyProperty.RegisterAttached("IsYear", typeof(bool), typeof(DatePickerCalendar), new PropertyMetadata(false, new PropertyChangedCallback(OnIsMonthYearChanged)));



        public static readonly DependencyProperty IsMonthYearProperty =
        DependencyProperty.RegisterAttached("IsMonthYear", typeof(bool), typeof(DatePickerCalendar),
                                            new PropertyMetadata(OnIsMonthYearChanged));

        public static bool GetIsMonthYear(DependencyObject dobj)
        {
            return (bool)dobj.GetValue(IsMonthYearProperty);
        }

        public static void SetIsMonthYear(DependencyObject dobj, bool value)
        {
            dobj.SetValue(IsMonthYearProperty, value);
        }

        private static void OnIsMonthYearChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs e)
        {
            var datePicker = (DatePicker)dobj;

            Application.Current.Dispatcher
                .BeginInvoke(DispatcherPriority.Loaded,
                             new Action<DatePicker, DependencyPropertyChangedEventArgs>(SetCalendarEventHandlers),
                             datePicker, e);
        }

        private static void SetCalendarEventHandlers(DatePicker datePicker, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
                return;

            if ((bool)e.NewValue)
            {
                datePicker.CalendarOpened += DatePickerOnCalendarOpened;
                datePicker.CalendarClosed += DatePickerOnCalendarClosed;
            }
            else
            {
                datePicker.CalendarOpened -= DatePickerOnCalendarOpened;
                datePicker.CalendarClosed -= DatePickerOnCalendarClosed;
            }
        }

        private static void DatePickerOnCalendarOpened(object sender, RoutedEventArgs routedEventArgs)
        {
            var calendar = GetDatePickerCalendar(sender);
            if (GetIsYear(sender as DatePicker))
            {
                calendar.DisplayMode = CalendarMode.Decade;
            }
            else
            {
                calendar.DisplayMode = CalendarMode.Year;
            }

            calendar.DisplayModeChanged += CalendarOnDisplayModeChanged;
        }

        private static void DatePickerOnCalendarClosed(object sender, RoutedEventArgs routedEventArgs)
        {
            var datePicker = (DatePicker)sender;
            var calendar = GetDatePickerCalendar(sender);
            datePicker.SelectedDate = calendar.SelectedDate;

            calendar.DisplayModeChanged -= CalendarOnDisplayModeChanged;
        }

        private static void CalendarOnDisplayModeChanged(object sender, CalendarModeChangedEventArgs e)
        {
            var calendar = (Calendar)sender;
            var datePicker = GetCalendarsDatePicker(calendar);

            bool mode = (GetIsYear(datePicker) && calendar.DisplayMode != CalendarMode.Year) || (GetIsMonthYear(datePicker) && calendar.DisplayMode != CalendarMode.Month);

            if (mode)
                return;

            calendar.SelectedDate = GetSelectedCalendarDate(calendar.DisplayDate);

            datePicker.IsDropDownOpen = false;
        }

        private static Calendar GetDatePickerCalendar(object sender)
        {
            var datePicker = (DatePicker)sender;
            var popup = (Popup)datePicker.Template.FindName("PART_Popup", datePicker);
            return ((Calendar)popup.Child);
        }

        private static DatePicker GetCalendarsDatePicker(FrameworkElement child)
        {
            var parent = (FrameworkElement)child.Parent;
            if (parent.Name == "PART_Root")
                return (DatePicker)parent.TemplatedParent;
            return GetCalendarsDatePicker(parent);
        }

        private static DateTime? GetSelectedCalendarDate(DateTime? selectedDate)
        {
            if (!selectedDate.HasValue)
                return null;
            return new DateTime(selectedDate.Value.Year, selectedDate.Value.Month, 1);
        }
    }
}
