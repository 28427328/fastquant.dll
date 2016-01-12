﻿// Copyright (c) FastQuant Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;

namespace SmartQuant
{
    public class GetByList<T> : IEnumerable<T>
    {
        private static MethodInfo nameMethodInfo;
        private static MethodInfo idMethodInfo;

        private Dictionary<string, T> dictionary;
        private IdArray<T> array;
        private List<T> list;

        public int Count => this.list.Count;

        public T this[int index]
        {
            get
            {
                return this.list[index];
            }
            set
            {
                this.list[index] = value;
            }
        }

        static GetByList()
        {
            nameMethodInfo = typeof(T).GetMethod("GetName", BindingFlags.NonPublic | BindingFlags.Instance);
            idMethodInfo = typeof(T).GetMethod("GetId", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public GetByList()
        {
            this.dictionary = new Dictionary<string, T>();
            this.array = new IdArray<T>();
            this.list = new List<T>();
        }

        public bool Contains(T obj)
        {
            string name = (string)nameMethodInfo?.Invoke(obj, new object[0]);
            return Contains(name);
        }

        public bool Contains(string name) => this.dictionary.ContainsKey(name);

        public bool Contains(int id) => this.array[id] != null;

        public void Add(T obj)
        {
            int id = (int)idMethodInfo.Invoke(obj, new object[0]);
            if (this.array[id] == null)
            {
                this.list.Add(obj);
                string name = (string)nameMethodInfo?.Invoke(obj, new object[0]);
                if (name != null)
                    this.dictionary[name] = obj;
                this.array[id] = obj;
            }
            else
                Console.WriteLine($"GetByList::Add Object with id = {id} is already in the list");
        }

        public void Remove(int id)
        {
            throw new NotImplementedException("don't use it");
        }

        public void Remove(T obj)
        {
            string name = (string)nameMethodInfo?.Invoke(obj, new object[0]);
            int? id = (int)idMethodInfo?.Invoke(obj, new object[0]);
            this.list.Remove(obj);
            if (name != null)
                this.dictionary.Remove(name);
            if (id != null)
                this.array.Remove((int)id);
        }

        public T GetByName(string name)
        {
            T obj;
            this.dictionary.TryGetValue(name, out obj);
            return obj;
        }

        public T GetByIndex(int index)
        {
            return this.list[index];
        }

        public T GetById(int id)
        {
            return this.array[id];
        }

        public void Clear()
        {
            this.dictionary.Clear();
            this.array.Clear();
            this.list.Clear();
        }

        public IEnumerator<T> GetEnumerator() => this.list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
