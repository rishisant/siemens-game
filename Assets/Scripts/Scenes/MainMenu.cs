using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void QuitGame(){
        Application.Quit();
    }

    public void Login()
    {
        // FIXME: make login sequence
        // get employee ID
        // hit the endpoint
        // have it return a session ID? or should we just validate everything with the employee ID?
        // employee ID might be easier, but session ID might prevent duplicate logins

        // need interface for making a new account

        // maybe just have it take in an employee ID and username, if it exists
        // then just log them in, otherwise if the employee ID is right then
        // create a new account

        // I should be able to hit the endpoint passing in username, password, employeeID and then get back either an OK or a session ID

        // check this out for implementation https://alialhaddad.medium.com/how-to-fetch-data-in-c-net-core-ea1ab720e3f9

    }
}
