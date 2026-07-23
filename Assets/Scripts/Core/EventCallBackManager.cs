using System;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

class EventItemType{
    public UnityObject owner;
    public Action<object> function;
}

class EventCallBackType{
    public string key;
    public List<EventItemType> events = new List<EventItemType>();
}

public class EventCallBackManager
{
    public delegate void EventCallBack();

    private List<EventCallBackType> events = new List<EventCallBackType>();
    private List<EventCallBack> callbacks = new List<EventCallBack>();

    public void subscribeEvent(string key, UnityObject owner, Action<object> func){
        if(func == null){
            return;
        }

        EventCallBackType eventGroup = events.Find(item => item.key == key);

        if(eventGroup == null){
            eventGroup = new EventCallBackType();
            eventGroup.key = key;
            events.Add(eventGroup);
        }

        eventGroup.events.Add(new EventItemType { owner = owner, function = func });
    }

    public void subscribeEvent(string key, Action<object> func){
        subscribeEvent(key, null, func);
    }

    public void removeEvent(string key, UnityObject owner){
        EventCallBackType eventGroup = events.Find(item => item.key == key);

        if(eventGroup == null){
            return;
        }

        eventGroup.events.RemoveAll(item => item.owner == owner);
    }

    public void addEvent(EventCallBack func){
        if(func == null){
            return;
        }

        callbacks.Add(func);
    }

    public void removeEvent(EventCallBack func){
        callbacks.Remove(func);
    }

    public void trigger(string key, object data = null){
        EventCallBackType eventGroup = events.Find(item => item.key == key);

        if(eventGroup == null){
            return;
        }

        for(int i = eventGroup.events.Count - 1; i >= 0; i--){
            EventItemType item = eventGroup.events[i];

            if(item.function == null || (item.owner != null && item.owner == null)){
                eventGroup.events.RemoveAt(i);
                continue;
            }

            item.function.Invoke(data);
        }
    }

    public void trigger(){
        for(int i = callbacks.Count - 1; i >= 0; i--){
            EventCallBack callback = callbacks[i];

            if(callback == null){
                callbacks.RemoveAt(i);
                continue;
            }

            callback.Invoke();
        }
    }
}
