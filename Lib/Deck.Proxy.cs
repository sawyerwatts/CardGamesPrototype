using System.Collections;

namespace CardGamesPrototype.Lib;

public partial class Deck
{
    public IEnumerator<Card> GetEnumerator()
    {
        return _cards.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_cards).GetEnumerator();
    }

    public void Add(Card item)
    {
        _cards.Add(item);
    }

    public void Clear()
    {
        _cards.Clear();
    }

    public bool Contains(Card item)
    {
        return _cards.Contains(item);
    }

    public void CopyTo(Card[] array, int arrayIndex)
    {
        _cards.CopyTo(array, arrayIndex);
    }

    public bool Remove(Card item)
    {
        return _cards.Remove(item);
    }

    public int Count => _cards.Count;

    public bool IsReadOnly => ((ICollection<Card>)_cards).IsReadOnly;

    public int IndexOf(Card item)
    {
        return _cards.IndexOf(item);
    }

    public void Insert(int index, Card item)
    {
        _cards.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        _cards.RemoveAt(index);
    }

    public Card this[int index]
    {
        get => _cards[index];
        set => _cards[index] = value;
    }
}
