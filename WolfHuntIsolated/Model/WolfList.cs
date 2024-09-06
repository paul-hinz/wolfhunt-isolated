using System.Collections.Generic;

namespace WolfHuntIsolated.Model;

public class WolfList
{
    private readonly List<Wolf> _members = new List<Wolf>();

    public Wolf GetLeft(Wolf wolf)
    {
        if (_members.Count == 1) return null;
        var ownInd = _members.IndexOf(wolf);
        if (ownInd == _members.Count - 1) return _members[0];
        return _members[ownInd + 1];
    }
    
    public Wolf GetRight(Wolf wolf)
    {
        if (_members.Count <= 2) return null;
        var ownInd = _members.IndexOf(wolf);
        if (ownInd == 0) return _members[^1];
        return _members[ownInd - 1];
    }

    public void Register(Wolf wolf)
    {
        _members.Add(wolf);
        UpdateList();
    }

    public void UpdateList()
    {
        _members.Sort();
    }
    
}