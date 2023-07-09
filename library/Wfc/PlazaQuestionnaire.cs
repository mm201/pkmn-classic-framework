using System;
using System.IO;

namespace PkmnFoundations.Wfc
{
  public class PlazaQuestionnaire
  {
    public PlazaQuestion CurrentQuestion;
    public PlazaQuestion LastWeeksQuestion;
    private int[] lastWeeksResults;
    public byte[] Unk;

    public PlazaQuestionnaire(PlazaQuestion currentQuestion, PlazaQuestion lastQuestion, int[] results, byte[] unk)
    {
      CurrentQuestion = currentQuestion;
      LastWeeksQuestion = lastQuestion;
      LastWeeksResults = results;
      Unk = unk;
    }

    public int[] LastWeeksResults
    {
      get
      {
        return lastWeeksResults;
      }

      set
      {
        if (value.Length != 3)
        {
          throw new ArgumentException("Results must be 3 integers! which represent the total count of each answer!");
        }
        lastWeeksResults = value;
      }
    }

    public byte[] Save()
    {
      byte[] serialized = new byte[728];
      MemoryStream ms = new MemoryStream(serialized);
      // BinaryWriter uses little endian which is what we want.
      BinaryWriter writer = new BinaryWriter(ms);

      writer.Write(CurrentQuestion.Save());
      writer.Write(LastWeeksQuestion.Save());
      foreach (int answer in LastWeeksResults)
      {
        writer.Write(answer);
      }
      writer.Write(Unk);

      writer.Flush();
      ms.Flush();
      return serialized;
    }

    public static PlazaQuestionnaire Load(byte[] data, int start)
    {
      PlazaQuestion question = PlazaQuestion.Load(data, start);
      PlazaQuestion lastWeeksQuestion = PlazaQuestion.Load(data, start + 352);
      int[] lastResults = new int[3];

      int dataIdx = 704;
      for (byte idx = 0; idx < 3; ++idx)
      {
        lastResults[idx] = BitConverter.ToInt32(data, start + dataIdx);
        dataIdx += 4;
      }

      return new PlazaQuestionnaire(
        question,
        lastWeeksQuestion,
        lastResults,
        new byte[] {
          data[start + dataIdx],
          data[start + dataIdx + 1],
          data[start + dataIdx + 2],
          data[start + dataIdx + 3],
          data[start + dataIdx + 4],
          data[start + dataIdx + 5],
          data[start + dataIdx + 6],
          data[start + dataIdx + 7],
          data[start + dataIdx + 8],
          data[start + dataIdx + 9],
          data[start + dataIdx + 10],
          data[start + dataIdx + 11],
        });
    }
  }

  /// <summary>
  /// A question that can be sent to a client for answering within the Wifi-Plaza.
  /// </summary>
  public class PlazaQuestion
  {
    /// <summary>
    /// Seems to be an internal ID as it's much higher than the week number should too the device
    /// from the responses we have captured.
    ///
    /// For us we just keep the IDs the same.
    ///
    /// The ID needs to be bigger than 1000 to show 'custom user text'.
    /// Otherwise, it overwrites the answers
    /// </summary>
    public int ID;
    /// <summary>The public ID, or week number shown to devices.</summary>
    public int PublicID;
    /// <summary>
    /// The sentence of the question, to actually show.
    ///
    /// Although the final question can't be more than 220 bytes encoded (110 characters since it's UTF-16).
    /// Although in reality, each line can only be 35 characters before needing a 'new line', spanned across
    /// two lines.
    ///
    /// Extra newlines are ignored.
    /// </summary>
    private string QuestionSentence;
    /// <summary>
    /// The three answers to the question.
    ///
    /// Each answer should be 36 bytes encoded (18 characters since it's UTF-16).
    /// If there is an unprintable character, they repeat the last printable character.
    /// Answers should not have newlines otherwise they can overwrite other lines.
    /// </summary>
    private string[] QuestionAnswers;
    /// <summary>A series of unknown bytes.</summary>
    public byte[] Unk;
    /// <summary>If the question is a 'special' question, and the man in the plaza will say so.</summary>
    public bool IsSpecial;

    public PlazaQuestion(int id, string sentence, string[] answers, byte[] unk, bool isSpecial)
    {
      ID = id;
      PublicID = id;
      Unk = unk;
      Sentence = sentence;
      Answers = answers;
      IsSpecial = isSpecial;
    }

    private PlazaQuestion(int id, int publicID, string sentence, string[] answers, byte[] unk, bool isSpecial)
    {
      ID = id;
      PublicID = publicID;
      Unk = unk;
      QuestionSentence = sentence;
      QuestionAnswers = answers;
      IsSpecial = isSpecial;
    }

    public string Sentence
    {
      get
      {
        return QuestionSentence;
      }

      set
      {
        // TODO add some validation on max length here.
        QuestionSentence = value;
      }
    }

    public string[] Answers
    {
      get
      {
        return QuestionAnswers;
      }

      set
      {
        if (value.Length != 3)
        {
          throw new ArgumentException("You MUST supply 3 answers for a particular question!");
        }
        // TODO validate encoded size
        QuestionAnswers = value;
      }
    }

    public byte[] Save()
    {
      byte[] serialized = new byte[352];
      MemoryStream ms = new MemoryStream(serialized);
      // BinaryWriter uses little endian which is what we want.
      BinaryWriter writer = new BinaryWriter(ms);

      writer.Write(ID);
      writer.Write(PublicID);

      byte[] encodedQuestion = Support.EncodedString4.EncodeString_impl(QuestionSentence, 220);
      writer.Write(encodedQuestion);
      foreach (string answer in QuestionAnswers)
      {
        byte[] encodedAnswer = Support.EncodedString4.EncodeString_impl(answer, 36);
        writer.Write(encodedAnswer);
      }
      writer.Write(Unk);
      writer.Write((int)(IsSpecial ? 1 : 0));

      writer.Flush();
      ms.Flush();
      return serialized;
    }

    public static PlazaQuestion Load(byte[] data, int start)
    {
      int internalID = BitConverter.ToInt32(data, start);
      int publicID = BitConverter.ToInt32(data, start + 4);

      byte[] questionBytes = new byte[220];
      Array.Copy(data, 8 + start, questionBytes, 0, 220);
      string question = Support.EncodedString4.DecodeString_impl(questionBytes);

      string[] answers = new string[3];
      int dataIdx = 228 + start;
      for (byte idx = 0; idx < 3; idx++)
      {
        byte[] answerBytes = new byte[36];
        Array.Copy(data, dataIdx, answerBytes, 0, 36);
        answers[idx] = Support.EncodedString4.DecodeString_impl(answerBytes);
        dataIdx += 36;
      }

      byte[] unk = new byte[] {
        data[start + 336], data[start + 337], data[start + 338], data[start + 339], data[start + 340],
        data[start + 341], data[start + 342], data[start + 343], data[start + 344], data[start + 345],
        data[start + 346], data[start + 347],
      };
      bool isSpecial = BitConverter.ToInt32(data, start + 348) != 0;

      return new PlazaQuestion(internalID, publicID, question, answers, unk, isSpecial);
    }
  }

  public class SubmittedQuestionnaire
  {
    public int ID;
    public int PublicID;
    private int answerNo;
    public uint OT;
    public Structures.TrainerGenders TrainerGender;
    public uint Country;
    public uint Region;

    public SubmittedQuestionnaire(int id, int publicID, int answerNumber, uint ot, Structures.TrainerGenders gender, uint country, uint region)
    {
      ID = id;
      PublicID = publicID;
      AnswerNumber = answerNumber;
      OT = ot;
      TrainerGender = gender;
      Country = country;
      Region = region;
    }

    public int AnswerNumber
    {
      get
      {
        return answerNo;
      }

      set
      {
        if (value > 3 || value < 0)
        {
          throw new ArgumentException("Answer can only be 0-3!");
        }
        answerNo = value;
      }
    }

    public byte[] Save()
    {
      byte[] serialized = new byte[24];
      MemoryStream ms = new MemoryStream(serialized);
      // BinaryWriter uses little endian which is what we want.
      BinaryWriter writer = new BinaryWriter(ms);

      writer.Write(ID);
      writer.Write(PublicID);
      writer.Write(answerNo);
      writer.Write(OT);
      writer.Write((int)TrainerGender);
      writer.Write(Country);
      writer.Write(Region);

      writer.Flush();
      ms.Flush();
      return serialized;
    }

    public static SubmittedQuestionnaire Load(byte[] data, int start)
    {
      int id = BitConverter.ToInt32(data, start);
      int publicId = BitConverter.ToInt32(data, start + 4);
      int answerNo = BitConverter.ToInt32(data, start + 8);
      uint ot = BitConverter.ToUInt32(data, start + 12);
      int genderNum = BitConverter.ToInt32(data, start + 16);
      ushort country = BitConverter.ToUInt16(data, start + 20);
      ushort region = BitConverter.ToUInt16(data, start + 22);

      return new SubmittedQuestionnaire(id, publicId, answerNo, ot, (Structures.TrainerGenders)genderNum, country, region);
    }
  }
}
