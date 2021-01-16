using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace EleGantt.core.utils
{
    // Example of use 
    //
    //  var models = new ObservableCollection<Model>();
    //  var viewModels =
    //    new BoundObservableCollection<ViewModel, Model>(
    //        models,
    //        m => new ViewModel(m), // creates a ViewModel from a Model
    //        (vm, m) => vm.Model.Equals(m)); // checks if the ViewModel corresponds to the specified model
    //
    // ref https://stackoverflow.com/questions/2853276/wpf-list-of-viewmodels-bound-to-list-of-model-objects
    // 
    // This version is an adaption to two way binding of the code show in the link above

    /// <summary>
    /// This class is used to synchronize two collections of different types, it's a two way binding
    /// </summary>
    /// <typeparam name="T">The target's collection type</typeparam>
    /// <typeparam name="TSource">The source's collection type</typeparam>
    public class BoundObservableCollection<T, TSource> : ObservableCollection<T>
    {
        private ObservableCollection<TSource> _source;
        private Func<TSource, T> _converter;
        private Func<T, TSource> _converterBack;
        private Func<T, TSource, bool> _isSameSource;

        public BoundObservableCollection(
            ObservableCollection<TSource> source,
            Func<TSource, T> converter,
            Func<T, TSource> converterBack,
            Func<T, TSource, bool> isSameSource)
            : base()
        {
            _source = source;
            _converter = converter;
            _converterBack = converterBack;
            _isSameSource = isSameSource;

            // Copy items
            AddItems(_source);

            // Subscribe to the CollectionChanged event
            CollectionChanged += new NotifyCollectionChangedEventHandler(_target_CollectionChanged);
        }

        /// <summary>
        /// Allow to convert and add an item back in the target's list
        /// </summary>
        /// <param name="items">List of items to add</param>
        /// <param name="index"></param>
        private void AddItems(IEnumerable<TSource> items, int index = -1)
        {
            foreach (var sourceItem in items)
            {
                Add(_converter(sourceItem));
            }
        }

        /// <summary>
        /// Allow to convert and add an item back in the source's list
        /// </summary>
        /// <param name="items">List of items to add back</param>
        /// <param name="index">Specify the insertion's index</param>
        private void AddItemsBack(IEnumerable<T> items, int index = -1)
        {
            if (index == -1) index = _source.Count();
            foreach (var sourceItem in items)
            {
                _source.Insert(index,_converterBack(sourceItem));
            }
        }

        /// <summary>
        /// This function will automatically update source list based on target's events
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event</param>
        void _target_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddItemsBack(e.NewItems.Cast<T>(), e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Move: // In reality move is never fired, the object is delete and added on the correct index
                    _source.Move(e.OldStartingIndex, e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var sourceItem in e.OldItems.Cast<T>())
                    {
                        var toRemove = _source.First(item => _isSameSource(sourceItem,item));
                        _source.Remove(toRemove);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    for (int i = e.NewStartingIndex; i < e.NewItems.Count; i++)
                    {
                        _source[i] = _converterBack((T)e.NewItems[i]);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    _source.Clear();
                    AddItemsBack(this);
                    break;
                default:
                    break;
            }
        }

    }
}
