using System.Collections;

namespace CardGamesPrototype.Lib.Common;

public partial class Deck
{
    public IEnumerator<CardState> GetEnumerator()
    {
        return _cards.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_cards).GetEnumerator();
    }

    public void Add(CardState item)
    {
        _cards.Add(item);
    }

    public void Clear()
    {
        _cards.Clear();
    }

    public bool Contains(CardState item)
    {
        return _cards.Contains(item);
    }

    public void CopyTo(CardState[] array, int arrayIndex)
    {
        _cards.CopyTo(array, arrayIndex);
    }

    public bool Remove(CardState item)
    {
        return _cards.Remove(item);
    }

    public int Count => _cards.Count;

    public bool IsReadOnly => ((ICollection<CardState>)_cards).IsReadOnly;

    public int IndexOf(CardState item)
    {
        return _cards.IndexOf(item);
    }

    public void Insert(int index, CardState item)
    {
        _cards.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        _cards.RemoveAt(index);
    }

    public CardState this[int index]
    {
        get => _cards[index];
        set => _cards[index] = value;
    }
}
