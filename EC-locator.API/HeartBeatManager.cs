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
    private string _apiUrl;

    private bool _runHeartBeat;
    
    public HeartBeatManager()
    {
        // URL with authentication and test ID
        _apiUrl = "https://push.statuscake.com/?PK=e663b671583f50e&TestID=6770491&time=0";
        
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
        if (_runHeartBeat)
        {
            Console.WriteLine("heartBeat Allready running!");
            return;
        }

        Thread heartBeat = new Thread(HeartBeat);
        _runHeartBeat = true;
        Console.WriteLine("start hb");
        heartBeat.Start();
    }

    public void StopHeartBeat()
    {
        Console.WriteLine("stop hb");
        _runHeartBeat = false;
    }

    private void HeartBeat(){

        while (_runHeartBeat)
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
            Thread.Sleep(1000);
        }
    
    }
}