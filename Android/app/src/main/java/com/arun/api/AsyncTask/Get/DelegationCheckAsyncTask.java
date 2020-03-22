package com.arun.api.AsyncTask.Get;

import android.os.AsyncTask;

import com.arun.api.Model.Delegation;
import com.arun.api.Model.DelegationDTO;

import org.json.JSONObject;

import java.io.BufferedInputStream;
import java.io.BufferedReader;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;

public class DelegationCheckAsyncTask extends AsyncTask<DelegationDTO, Void, String> {
    public IDelegationcallback icallback;

    public DelegationCheckAsyncTask(IDelegationcallback icallback) {
        this.icallback = icallback;
    }
    @Override
    protected String doInBackground(DelegationDTO... delegationDTO) {
        String empId = String.valueOf(delegationDTO[0].getDelegatedTo().getUserName());
        String fromDate = String.valueOf(delegationDTO[0].getFromDate());
        String toDate = String.valueOf(delegationDTO[0].getToDate());

        JSONObject jsonObj = null;
        StringBuilder response = new StringBuilder();
        try {
            URL url = new URL("http://10.0.2.2:1256/api/Employee/CheckDelegation?empname=" + empId + "&FromDate=" + fromDate+"&ToDate="+toDate);
            HttpURLConnection conn = (HttpURLConnection) url.openConnection();
            conn.setRequestMethod("GET");
            try {
                InputStream inputStream = new BufferedInputStream(conn.getInputStream());
                BufferedReader r = new BufferedReader(new InputStreamReader(inputStream));
                for (String line; (line = r.readLine()) != null; ) {
                    response.append(line);
                }
            } catch (Exception e) {
                e.printStackTrace();
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
        return  response.toString();
    }


    @Override
    protected void onPostExecute(String string) {
        if (string == null)
            icallback.getStatus(-1);
        try {
            int status = Integer.parseInt(string) ;
            icallback.getStatus(status);
        } catch (Exception e) {
        }

    }

    public interface IDelegationcallback {
        void getStatus(int status);
    }
}