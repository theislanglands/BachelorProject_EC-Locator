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
    private TimeSpan _heartBeatRate;
    
    
    private bool _heartBeatRunning;
    
    public HeartBeatManager(string apiUrl)
    {
        _apiUrl = apiUrl;
        _heartBeatRate = new TimeSpan(0, 0, 1);
        
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
            /*
            // Send the request and handle the response
            HttpResponseMessage response = _httpClient.GetAsync(_apiUrl).Result;

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Heartbeat sent successfully.");
            }
    
            else
            {
                Console.WriteLine("Error sending heartbeat: " + response.StatusCode);
            }
            */
            
            Console.WriteLine("test of HeartBeatthreat");
            Thread.Sleep(_heartBeatRate);
        }
    
    }
}