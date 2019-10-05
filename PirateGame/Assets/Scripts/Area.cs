using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Area : MonoBehaviour
{
    public UnityEvent OnTEnter;
    public List<GameObject> UsersInArea = new List<GameObject>();
    public enum e_Location
    {
        UPPERDESK,
        LOWERDESK,
        BUCKETAREA,
        SAILAREA,
        BORDERAREA,
        CANONAREA,
        ANCHORAREA
    }
    public e_Location Location = e_Location.UPPERDESK;


    private void OnTriggerEnter(Collider other)
    {
        
        if (other.GetComponent<Crew>())
        {
            
            Crew c = other.GetComponent<Crew>();
            if(c.Destination==Location.ToString())
            {
                OnTEnter.Invoke();
                if (!UsersInArea.Contains(other.gameObject))
                {
                    UsersInArea.Add(other.gameObject);
                }
                
                //c.CurrentArea.UsersInArea.Remove(other.gameObject);
                c.CurrentArea = this.gameObject.GetComponent<Area>();
                c.CurrentArea.Location = this.gameObject.GetComponent<Area>().Location;
                c.EnteredArea.Invoke();
                c.LastLocation = this.gameObject.GetComponent<Area>().Location.ToString();

                for(int i=0;i<UsersInArea.Count;i++)
                {
                    if (UsersInArea[i].GetComponent<Crew>().CurrentArea.Location.ToString() !=
                        this.gameObject.GetComponent<Area>().Location.ToString())
                    {
                        UsersInArea.RemoveAt(i);
                    }
                }

                switch (Location)
                {
                    case e_Location.UPPERDESK:
                        c.Location = Crew.e_Location.UPPERDESK;
                        break;
                    case e_Location.LOWERDESK:
                        c.Location = Crew.e_Location.LOWERDESK;
                        break;
                    case e_Location.BUCKETAREA:
                        c.Location = Crew.e_Location.BUCKETAREA;
                        break;
                    case e_Location.SAILAREA:
                        c.Location = Crew.e_Location.SAILAREA;
                        break;
                    case e_Location.BORDERAREA:
                        c.Location = Crew.e_Location.BORDERAREA;
                        break;
                    case e_Location.CANONAREA:
                        c.Location = Crew.e_Location.CANONAREA;
                        break;
                    case e_Location.ANCHORAREA:
                        c.Location = Crew.e_Location.ANCHORAREA;
                        break;
                }
            }
        }
    }
    
}
