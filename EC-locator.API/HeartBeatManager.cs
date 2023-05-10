using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace API;

public class HeartBeatManager
{
    private readonly HttpClient? _httpClient;
    private readonly string _apiUrl;
    private readonly TimeSpan _heartBeatRate;
    
    
    private bool _heartBeatRunning;
    
    public HeartBeatManager(string apiUrl, string rate)
    {
        _apiUrl = apiUrl;
        var times = rate.Split(":");
        _heartBeatRate = new TimeSpan(int.Parse(times[0]), int.Parse(times[1]), int.Parse(times[2]), int.Parse(times[3]));
        
        if (_httpClient == null)
        {
            // Create the HTTP client
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_apiUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
        }
    }

    public void StartHeartBeat()
    {
        if (_heartBeatRunning)
        {
            return;
        }

        Thread heartBeat = new Thread(HeartBeat);
        _heartBeatRunning = true;
        Console.WriteLine("start hb");
        heartBeat.Start();
    }

    public void StopHeartBeat()
    {
        Console.WriteLine("stop hb");
        _heartBeatRunning = false;
    }

    private void HeartBeat(){

        while (_heartBeatRunning)
        {
            try
            {
                HttpResponseMessage response = _httpClient.GetAsync(_apiUrl).Result;

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Heartbeat sent successfully.");
                }
                else
                {
                    Console.WriteLine("Error sending heartbeat: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error sending Heartbeat"+ ex.Message);
            }
            
            // Console.WriteLine("test of HeartBeatthreat");
            Thread.Sleep(_heartBeatRate);
        }
    }
}