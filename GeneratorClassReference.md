# WARNING #

This material is provided for reference only. Generator is in active development now.

# Generator Class #

To achieve metageneration metacreator creates special class that is able to generate code you want. This class in general have the next structure:

```

public Generator : IGenerator, IMetaWriter
{
  readonly Engine Engine;

  public Generator(Engine engine)
  {
    Engine = engine;
  }

  public string Run()
  {
    // some code
  }
}

```

Full current version you can find at
http://code.google.com/p/metacreator/source/browse/MetaCreator/Evaluation/_GeneratorSkeleton.cs

# Members #

...

# How metablocks are expanded #