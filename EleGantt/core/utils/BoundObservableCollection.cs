using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
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

        private void AddItems(IEnumerable<TSource> items)
        {
            foreach (var sourceItem in items)
            {
                Add(_converter(sourceItem));
            }
        }

        private void AddItemsBack(IEnumerable<T> items, int index = -1)
        {
            if (index == -1) index = _source.Count();
            foreach (var sourceItem in items)
            {
                _source.Insert(index,_converterBack(sourceItem));
            }
        }

        void _target_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddItemsBack(e.NewItems.Cast<T>(), e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Move:
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

        void _source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddItems(e.NewItems.Cast<TSource>());
                    break;
                case NotifyCollectionChangedAction.Move:
                    Move(e.OldStartingIndex, e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var sourceItem in e.OldItems.Cast<TSource>())
                    {
                        var toRemove = this.First(item => _isSameSource(item, sourceItem));
                        Remove(toRemove);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    for (int i = e.NewStartingIndex; i < e.NewItems.Count; i++)
                    {
                        this[i] = _converter((TSource)e.NewItems[i]);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Clear();
                    AddItems(_source);
                    break;
                default:
                    break;
            }
        }

    }
}
