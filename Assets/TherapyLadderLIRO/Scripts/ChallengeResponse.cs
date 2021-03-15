using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ChallengeResponse {
    public long m_challengeID;
    public int m_cycle;
    public int m_block;
    public int m_presentationNumber;
    public int m_lexicalPresentationNumber;
    public int m_basketNumber;
    public DateTime m_dateTimeStart;
    public DateTime m_dateTimeEnd;
    public List<string> incorrectPicturesIDs = new List<string>();
    public int m_accuracy;
    public int m_repeat;
}

public class ChallengeResponseACT {

    public long m_challengeID;
    public DateTime m_timeStamp;
    public int m_number;
    public int m_block;
    public int m_cycle;
    public int m_accuracy;
    public float m_reactionTime;
    public int m_repeat;
    public int m_pictureID;

}

