﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Net.Http.Headers;
using System.Net;

namespace Hawaso.Rules;

#region RedirectAzureWebsitesRule: 메인(닷컴) 도메인으로 이동시키기
/// <summary>
/// 메인(닷컴) 도메인으로 이동시키기 
/// "hawaso.azurewebsites.net" 요청시 "www.hawaso.com" 경로로 이동 
/// </summary>
public class RedirectAzureWebsitesRule : IRule
{
    public int StatusCode { get; } = (int)HttpStatusCode.MovedPermanently;

    public void ApplyRule(RewriteContext context)
    {
        HttpRequest request = context.HttpContext.Request;
        HostString host = context.HttpContext.Request.Host;

        if (host.HasValue && host.Value == "hawaso.azurewebsites.net")
        {
            HttpResponse response = context.HttpContext.Response;
            response.StatusCode = StatusCode;
            response.Headers[HeaderNames.Location] = request.Scheme + "://" + "www.hawaso.com" + request.PathBase + request.Path + request.QueryString;
            context.Result = RuleResult.EndResponse;
        }
        else
        {
            context.Result = RuleResult.ContinueRules;
        }
    }
}
#endregion
