﻿global using Autofac;
global using Autofac.Extensions.DependencyInjection;
global using HealthChecks.UI.Client;
global using Microsoft.AspNetCore;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Diagnostics.HealthChecks;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.SignalR;
global using Microsoft.eShopOnContainers.BuildingBlocks.EventBus;
global using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
global using Microsoft.eShopOnContainers.BuildingBlocks.EventBusRabbitMQ;
global using Microsoft.eShopOnContainers.BuildingBlocks.EventBusServiceBus;
global using Microsoft.eShopOnContainers.Services.Ordering.SignalrHub;
global using Microsoft.eShopOnContainers.Services.Ordering.SignalrHub.AutofacModules;
global using Microsoft.eShopOnContainers.Services.Ordering.SignalrHub.IntegrationEvents;
global using Microsoft.eShopOnContainers.Services.Ordering.SignalrHub.IntegrationEvents.EventHandling;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using Microsoft.Extensions.Logging;
global using RabbitMQ.Client;
global using Serilog;
global using Serilog.Context;
global using System;
global using System.IdentityModel.Tokens.Jwt;
global using System.IO;
global using System.Reflection;
global using System.Threading.Tasks;
