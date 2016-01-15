﻿using System;

namespace SmartQuant
{
    public class EventServer
    {
        private Framework framework;
        private EventBus bus;

        public EventServer(Framework framework, EventBus bus)
        {
            this.framework = framework;
            this.bus = bus;
        }

        public void OnProviderAdded(IProvider provider)
        {
            OnEvent(new OnProviderAdded(provider));
        }

        public void OnProviderRemoved(Provider provider)
        {
            OnEvent(new OnProviderRemoved(provider));
        }

        public void OnProviderConnected(Provider provider)
        {
            OnEvent(new OnProviderConnected(this.framework.Clock.DateTime, provider));
        }

        public void OnProviderDisconnected(Provider provider)
        {
            OnEvent(new OnProviderDisconnected(this.framework.Clock.DateTime, provider));
        }

        public void OnProviderError(ProviderError error)
        {
            OnEvent(error);
        }

        public void OnProviderStatusChanged(Provider provider)
        {
            switch (provider.Status)
            {
                case ProviderStatus.Connected:
                    OnProviderConnected(provider);
                    break;
                case ProviderStatus.Disconnected:
                    OnProviderDisconnected(provider);
                    break;
            }
            OnEvent(new OnProviderStatusChanged(provider));
        }

        internal void OnPortfolioParentChanged(Portfolio portfolio, bool v)
        {
            throw new NotImplementedException();
        }

        public void OnEvent(Event e)
        {
            this.framework.EventManager.OnEvent(e);
        }

        public void OnInstrumentAdded(Instrument instrument) => OnEvent(new OnInstrumentAdded(instrument));

        public void OnInstrumentDefinition(InstrumentDefinition definition) => OnEvent(new OnInstrumentDefinition(definition));

        public void OnInstrumentDefintionEnd(InstrumentDefinitionEnd end) => OnEvent(new OnInstrumentDefinitionEnd(end));

        public void OnInstrumentDeleted(Instrument instrument) => OnEvent(new OnInstrumentDeleted(instrument));

        public void OnStrategyAdded(Strategy strategy) => OnEvent(new OnStrategyAdded(strategy));

        internal void OnPositionChanged(Portfolio portfolio, Position position, bool queued)
        {
            throw new NotImplementedException();
        }

        public void OnLog(Event e)
        {
            OnEvent(e);
        }

        internal void OnPositionOpened(Portfolio portfolio, Position position, bool queued)
        {
            throw new NotImplementedException();
        }

        internal void OnTransaction(Portfolio portfolio, Transaction transaction, bool queued)
        {
            throw new NotImplementedException();
        }

        internal void OnFill(Portfolio portfolio, Fill fill, bool queued)
        {
            throw new NotImplementedException();
        }

        internal void OnPositionClosed(Portfolio portfolio, Position position, bool queued)
        {
            throw new NotImplementedException();
        }

        public void OnFrameworkCleared(Framework framework)
        {
            OnEvent(new OnFrameworkCleared(framework));
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }
    }
}