﻿namespace SmartQuant
{
    public class PortfolioManager
    {
        private Framework framework;

        public PortfolioServer Server { get; set; }

        public PortfolioManager(Framework framework, PortfolioServer portfolioServer)
        {
            this.framework = framework;
            Server = portfolioServer;
        }

        public Pricer Pricer { get; internal set; }

    }
}