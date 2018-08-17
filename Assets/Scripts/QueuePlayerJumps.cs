using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueuePlayerJumps
{
    public List<PlayerJump> m_Queue;

    public bool IsEmpty { get { return ( m_Queue==null || m_Queue.Count<=0 ); } }

    public PlayerJump GetLast { get { return IsEmpty ? null : m_Queue[m_Queue.Count-1]; } }

    public QueuePlayerJumps()
    {
        m_Queue = new List<PlayerJump>();
    }

    public void EnqueueJump( PlayerJump pj )
    {
        m_Queue.Add(pj);
    }

    // RaycastHit? - ? потому что null он не хотел возвращать
    public PlayerJump DequeueJump()
    {
        if ( IsEmpty )
            return null;

        PlayerJump ret = m_Queue[0];
        m_Queue.RemoveAt(0);
        return ret;
    }
}