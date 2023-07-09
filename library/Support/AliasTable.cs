using System;
using System.Collections.Generic;

namespace PkmnFoundations.Support
{
  /// <summary>
  /// Vose's implementation of the Alias method for choosing weighted randomly from a set.
  ///
  /// See: https://www.keithschwarz.com/darts-dice-coins/
  /// </summary>
  public class AliasTable<Type>
  {
    public static AliasTable<Type> NewWithWeights(Dictionary<Type, double> typesWithProbabilities)
    {
      List<Type> elements = new List<Type>();
      foreach (var pair in typesWithProbabilities)
      {
        elements.Add(pair.Key);
      }
      Dictionary<Type, Type> table = new Dictionary<Type, Type>();
      Dictionary<Type, double> probs = new Dictionary<Type, double>();

      double size = (double)typesWithProbabilities.Count;
      Stack<Type> smallTypes = new Stack<Type>();
      Stack<Type> largeTypes = new Stack<Type>();
      Dictionary<Type, double> scaledProbabilityMap = new Dictionary<Type, double>();

      foreach (var pair in typesWithProbabilities)
      {
        double scaledProbability = pair.Value * size;
        scaledProbabilityMap[pair.Key] = scaledProbability;

        if (scaledProbability < 1.0)
        {
          smallTypes.Push(pair.Key);
        }
        else
        {
          largeTypes.Push(pair.Key);
        }
      }

      while (smallTypes.Count != 0 && largeTypes.Count != 0)
      {
        Type smallElement = smallTypes.Pop();
        Type largeElement = largeTypes.Pop();
        table[smallElement] = largeElement;

        double scaledSmall = scaledProbabilityMap[smallElement];
        double scaledLarge = scaledProbabilityMap[largeElement];
        probs[smallElement] = scaledSmall;
        double newLarge = (scaledLarge + scaledSmall) - 1.0;
        probs[largeElement] = newLarge;

        if (newLarge < 1.0)
        {
          smallTypes.Push(largeElement);
        }
        else
        {
          largeTypes.Push(largeElement);
        }
      }

      while (largeTypes.Count != 0)
      {
        Type largeElement = largeTypes.Pop();
        probs[largeElement] = 1.0;
      }

      while (smallTypes.Count != 0)
      {
        Type smallElement = smallTypes.Pop();
        probs[smallElement] = 1.0;
      }

      return new AliasTable<Type>(table, elements, probs);
    }

    public Type Sample()
    {
      Type element = elements[rng.Next(0, elements.Count)];
      int number = rng.Next(0, 101);

      double probability = probabilities[element];
      if (number <= (probability * 100))
      {
        return element;
      }
      else
      {
        return underlyingTable[element];
      }
    }

    private AliasTable(Dictionary<Type, Type> table, List<Type> elem, Dictionary<Type, double> probs)
    {
      underlyingTable = table;
      elements = elem;
      probabilities = probs;
      rng = new Random();
    }

    private Dictionary<Type, Type> underlyingTable;
    private List<Type> elements;
    private Dictionary<Type, double> probabilities;
    private Random rng;
  }
}
