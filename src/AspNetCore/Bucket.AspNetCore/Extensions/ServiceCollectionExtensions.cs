using Bucket.AspNetCore.Commons;
using Bucket.Core;
using Bucket.DependencyInjection;
using Bucket.ErrorCode;
using Bucket.Redis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bucket.AspNetCore.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 框架基础
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        [Obsolete("Please use AddBucketAspNetCore")]
        public static IServiceCollection AddBucket(this IServiceCollection services)
        {
            return AddBucketAspNetCore(services);
        }
        /// <summary>
        /// 添加框架默认基础
        /// AddOptions\AddLogging\AddHttpContextAccessor\AddIUser\AddIRequestScopedDataRepository\AddIJsonHelper\AddIErrorCode
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddBucketAspNetCore(this IServiceCollection services)
        {
            services.AddOptions();
            services.AddLogging();
            services.AddSingleton<RedisClient>();
            services.AddHttpContextAccessor();
            services.AddSingleton<IUser, HttpContextUser>();
            services.AddSingleton<IRequestScopedDataRepository, HttpDataRepository>();
            services.AddSingleton<IErrorCode, EmptyErrorCode>();
            // 关闭模型自动化验证,实现自控
            services.Configure<ApiBehaviorOptions>(opts => opts.SuppressModelStateInvalidFilter = true);
            return services;
        }
        /// <summary>
        /// 添加框架默认基础
        /// AddOptions\AddLogging\AddHttpContextAccessor\AddIUser\AddIRequestScopedDataRepository\AddIJsonHelper\AddIErrorCode
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAspNetCore(this IBucketBuilder builder)
        {
            return AddBucketAspNetCore(builder.Services);
        }

        /// <summary>
        /// 跨域服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                );
            });
            return services;
        }


        /// <summary>
        /// 批量注册服务
        /// </summary>
        /// <param name="services">DI服务</param>
        /// <param name="assemblys">需要批量注册的程序集集合</param>
        /// <param name="serviceLifetime">服务生命周期</param>
        /// <returns></returns>
        public static IServiceCollection BatchRegisterService(this IServiceCollection services, Assembly[] assemblys, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        {
            List<Type> typeList = new List<Type>();  // 所有符合注册条件的类集合
            foreach (var assembly in assemblys)
            {
                // 筛选当前程序集下符合条件的类
                var types = assembly.GetTypes().Where(t => !t.IsInterface && !t.IsSealed && !t.IsAbstract);
                if (types != null && types.Count() > 0)
                    typeList.AddRange(types);
            }
            if (typeList.Count() == 0)
                return services;

            var typeDic = new Dictionary<Type, Type[]>(); //待注册集合
            foreach (var type in typeList)
            {
                var interfaces = type.GetInterfaces();   //获取接口
                typeDic.Add(type, interfaces);
            }
            if (typeDic.Keys.Count() > 0)
            {
                foreach (var instanceType in typeDic.Keys)
                {
                    foreach (var interfaceType in typeDic[instanceType])
                    {
                        //根据指定的生命周期进行注册
                        switch (serviceLifetime)
                        {
                            case ServiceLifetime.Scoped:
                                services.AddScoped(interfaceType, instanceType);
                                break;
                            case ServiceLifetime.Singleton:
                                services.AddSingleton(interfaceType, instanceType);
                                break;
                            case ServiceLifetime.Transient:
                                services.AddTransient(interfaceType, instanceType);
                                break;
                        }
                    }
                }
            }
            return services;
        }
        /// <summary>
        /// 批量注册服务
        /// </summary>
        /// <param name="services">DI服务</param>
        /// <param name="assembly">需要批量注册的程序集</param>
        /// <param name="endWith">类名结束字符</param>
        /// <param name="serviceLifetime">服务生命周期</param>
        /// <returns></returns>
        public static IServiceCollection BatchRegisterService(this IServiceCollection services, Assembly assembly, string endWith, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        {
            IEnumerable<Type> typeList = assembly.GetTypes().Where(t => !t.IsInterface && !t.IsSealed && !t.IsAbstract && t.Name.EndsWith(endWith));  // 所有符合注册条件的类集合
            if (typeList.Count() == 0)
                return services;

            var typeDic = new Dictionary<Type, Type[]>(); // 待注册集合
            foreach (var type in typeList)
            {
                var interfaces = type.GetInterfaces();   // 获取接口
                typeDic.Add(type, interfaces);
            }
            if (typeDic.Keys.Count() > 0)
            {
                foreach (var instanceType in typeDic.Keys)
                {
                    foreach (var interfaceType in typeDic[instanceType])
                    {
                        //根据指定的生命周期进行注册
                        switch (serviceLifetime)
                        {
                            case ServiceLifetime.Scoped:
                                services.AddScoped(interfaceType, instanceType);
                                break;
                            case ServiceLifetime.Singleton:
                                services.AddSingleton(interfaceType, instanceType);
                                break;
                            case ServiceLifetime.Transient:
                                services.AddTransient(interfaceType, instanceType);
                                break;
                        }
                    }
                }
            }
            return services;
        }
        /// <summary>
        /// 批量注册服务
        /// </summary>
        /// <param name="services">DI服务</param>
        /// <param name="typeList">类集合</param>
        /// <param name="serviceLifetime">服务生命周期</param>
        /// <returns></returns>
        public static IServiceCollection BatchRegisterService(this IServiceCollection services, IEnumerable<Type> typeList, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        {
            if (typeList.Count() == 0)
                return services;

            var typeDic = new Dictionary<Type, Type[]>(); //待注册集合
            foreach (var type in typeList)
            {
                var interfaces = type.GetInterfaces();   //获取接口
                typeDic.Add(type, interfaces);
            }
            if (typeDic.Keys.Count() > 0)
            {
                foreach (var instanceType in typeDic.Keys)
                {
                    foreach (var interfaceType in typeDic[instanceType])
                    {
                        //根据指定的生命周期进行注册
                        switch (serviceLifetime)
                        {
                            case ServiceLifetime.Scoped:
                                services.AddScoped(interfaceType, instanceType);
                                break;
                            case ServiceLifetime.Singleton:
                                services.AddSingleton(interfaceType, instanceType);
                                break;
                            case ServiceLifetime.Transient:
                                services.AddTransient(interfaceType, instanceType);
                                break;
                        }
                    }
                }
            }
            return services;
        }


        /// <summary>
        /// 批量注册服务
        /// </summary>
        /// <param name="builder">DI服务</param>
        /// <param name="assemblys">需要批量注册的程序集集合</param>
        /// <param name="serviceLifetime">服务生命周期</param>
        /// <returns></returns>
        public static IServiceCollection BatchRegisterService(this IBucketBuilder builder, Assembly[] assemblys, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        {
            return BatchRegisterService(builder.Services, assemblys, serviceLifetime);
        }
        /// <summary>
        /// 批量注册服务
        /// </summary>
        /// <param name="services">DI服务</param>
        /// <param name="assembly">需要批量注册的程序集</param>
        /// <param name="endWith">类名结束字符</param>
        /// <param name="serviceLifetime">服务生命周期</param>
        /// <returns></returns>
        public static IServiceCollection BatchRegisterService(this IBucketBuilder builder, Assembly assembly, string endWith, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        {
            return BatchRegisterService(builder.Services, assembly, endWith, serviceLifetime);
        }
        /// <summary>
        /// 批量注册服务
        /// </summary>
        /// <param name="services">DI服务</param>
        /// <param name="typeList">类集合</param>
        /// <param name="serviceLifetime">服务生命周期</param>
        /// <returns></returns>
        public static IServiceCollection BatchRegisterService(this IBucketBuilder builder, IEnumerable<Type> typeList, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        {
            return BatchRegisterService(builder.Services, typeList, serviceLifetime);
        }
    }
}
