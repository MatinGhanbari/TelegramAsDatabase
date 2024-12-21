using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramAsDatabase.Models;

public class TDBRetryPolicies
{
    private int _retryCount = 10;

    public int RetryCount
    {
        get => _retryCount;
        set => _retryCount = value >= 0 ? value : 5;
    }
}