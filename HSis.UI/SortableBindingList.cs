#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace HSis.UI
{
    public class SortableBindingList<T> : BindingList<T>
    {
        private bool _isSorted;
        private ListSortDirection _sortDirection = ListSortDirection.Ascending;
        private PropertyDescriptor? _sortProperty;

        public SortableBindingList()
        {
        }

        public SortableBindingList(IList<T> list) : base(list)
        {
        }

        protected override bool SupportsSortingCore => true;

        protected override bool IsSortedCore => _isSorted;

        protected override ListSortDirection SortDirectionCore => _sortDirection;

        protected override PropertyDescriptor? SortPropertyCore => _sortProperty;

        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            _sortProperty = prop;
            _sortDirection = direction;

            var itemsList = this.Items as List<T>;
            if (itemsList == null)
            {
                itemsList = this.Items.ToList();
            }

            var pc = new PropertyComparer<T>(prop, direction);
            itemsList.Sort(pc);

            if (this.Items != itemsList)
            {
                this.RaiseListChangedEvents = false;
                this.Clear();
                foreach (var item in itemsList)
                {
                    this.Add(item);
                }
                this.RaiseListChangedEvents = true;
            }

            _isSorted = true;
            this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override void RemoveSortCore()
        {
            _isSorted = false;
            _sortProperty = null;
            _sortDirection = ListSortDirection.Ascending;
            this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }
    }

    public class PropertyComparer<T> : IComparer<T>
    {
        private readonly PropertyDescriptor _property;
        private readonly ListSortDirection _direction;

        public PropertyComparer(PropertyDescriptor property, ListSortDirection direction)
        {
            _property = property;
            _direction = direction;
        }

        public int Compare(T? x, T? y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return _direction == ListSortDirection.Ascending ? -1 : 1;
            if (y == null) return _direction == ListSortDirection.Ascending ? 1 : -1;

            var xValue = _property.GetValue(x);
            var yValue = _property.GetValue(y);

            return CompareValues(xValue, yValue, _direction);
        }

        private int CompareValues(object? xValue, object? yValue, ListSortDirection direction)
        {
            int result;

            if (xValue == null && yValue == null)
            {
                result = 0;
            }
            else if (xValue == null)
            {
                result = -1;
            }
            else if (yValue == null)
            {
                result = 1;
            }
            else if (xValue is IComparable comparableX)
            {
                result = comparableX.CompareTo(yValue);
            }
            else if (yValue is IComparable comparableY)
            {
                result = -comparableY.CompareTo(xValue);
            }
            else if (xValue.Equals(yValue))
            {
                result = 0;
            }
            else
            {
                result = string.Compare(xValue.ToString(), yValue.ToString(), StringComparison.OrdinalIgnoreCase);
            }

            if (direction == ListSortDirection.Descending)
            {
                result = -result;
            }

            return result;
        }
    }
}
