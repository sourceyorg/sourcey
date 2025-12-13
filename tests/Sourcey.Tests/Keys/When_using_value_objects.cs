using Shouldly;
using Sourcey.Keys;

namespace Sourcey.Tests.Keys;

public class When_using_value_objects
{
    [Then]
    public void Equality_and_hashcode_should_match_for_same_values()
    {
        // Arrange
        var a1 = Actor.From("a");
        var a2 = Actor.From("a");
        var e1 = EventId.From("e");
        var e2 = EventId.From("e");
        var s1 = StreamId.From("s");
        var s2 = StreamId.From("s");
        var sub1 = Subject.From("sub");
        var sub2 = Subject.From("sub");
        var c1 = Causation.From("c");
        var c2 = Causation.From("c");
        var corr1 = Correlation.From("corr");
        var corr2 = Correlation.From("corr");

        // Assert equality
        a1.ShouldBe(a2);
        e1.ShouldBe(e2);
        s1.ShouldBe(s2);
        sub1.ShouldBe(sub2);
        c1.ShouldBe(c2);
        corr1.ShouldBe(corr2);

        // Assert hash codes
        a1.GetHashCode().ShouldBe(a2.GetHashCode());
        e1.GetHashCode().ShouldBe(e2.GetHashCode());
        s1.GetHashCode().ShouldBe(s2.GetHashCode());
        sub1.GetHashCode().ShouldBe(sub2.GetHashCode());
        c1.GetHashCode().ShouldBe(c2.GetHashCode());
        corr1.GetHashCode().ShouldBe(corr2.GetHashCode());
    }

    [Then]
    public void Inequality_should_hold_for_different_values()
    {
        Actor.From("a").ShouldNotBe(Actor.From("b"));
        EventId.From("e1").ShouldNotBe(EventId.From("e2"));
        StreamId.From("s1").ShouldNotBe(StreamId.From("s2"));
        Subject.From("x").ShouldNotBe(Subject.From("y"));
        Causation.From("c1").ShouldNotBe(Causation.From("c2"));
        Correlation.From("k1").ShouldNotBe(Correlation.From("k2"));
    }

    [Then]
    public void Implicit_conversions_should_roundtrip()
    {
        // string -> value -> string
        Actor a = "act";
        string aStr = a;
        aStr.ShouldBe("act");

        EventId e = "evt";
        string eStr = e;
        eStr.ShouldBe("evt");

        StreamId s = "str";
        string sStr = s;
        sStr.ShouldBe("str");

        Subject sub = "subj";
        string subStr = sub;
        subStr.ShouldBe("subj");

        Causation c = "cause";
        string cStr = c;
        cStr.ShouldBe("cause");

        Correlation k = "corr";
        string kStr = k;
        kStr.ShouldBe("corr");
    }

    [Then]
    public void From_null_should_yield_Unknown_singletons()
    {
        Actor.From(null!).ShouldBe(Actor.Unknown);
        EventId.From(null!).ShouldBe(EventId.Unknown);
        StreamId.From(null!).ShouldBe(StreamId.Unknown);
        Subject.From(null!).ToString().ShouldBe(Subject.From(null!).ToString()); // Subject has no Unknown field
        Causation.From(null!).ShouldBe(Causation.Unknown);
        Correlation.From(null!).ShouldBe(Correlation.Unknown);
    }

    [Then]
    public void New_should_return_non_empty_and_distinct_values()
    {
        var a1 = Actor.New();
        var a2 = Actor.New();
        a1.ShouldNotBe(Actor.Unknown);
        a1.ToString().ShouldNotBeNullOrWhiteSpace();
        a2.ToString().ShouldNotBeNullOrWhiteSpace();
        a1.ShouldNotBe(a2);

        var e1 = EventId.New();
        var e2 = EventId.New();
        e1.ShouldNotBe(EventId.Unknown);
        e1.ShouldNotBe(e2);

        var s1 = StreamId.New();
        var s2 = StreamId.New();
        s1.ShouldNotBe(StreamId.Unknown);
        s1.ShouldNotBe(s2);

        var sub1 = Subject.New();
        var sub2 = Subject.New();
        sub1.ShouldNotBe(sub2);

        var c1 = Causation.New();
        var c2 = Causation.New();
        c1.ShouldNotBe(Causation.Unknown);
        c1.ShouldNotBe(c2);

        var k1 = Correlation.New();
        var k2 = Correlation.New();
        k1.ShouldNotBe(Correlation.Unknown);
        k1.ShouldNotBe(k2);
    }

    [Then]
    public void ToString_should_return_underlying_value()
    {
        Actor.From("a").ToString().ShouldBe("a");
        EventId.From("e").ToString().ShouldBe("e");
        StreamId.From("s").ToString().ShouldBe("s");
        Subject.From("sub").ToString().ShouldBe("sub");
        Causation.From("c").ToString().ShouldBe("c");
        Correlation.From("corr").ToString().ShouldBe("corr");
    }
}
