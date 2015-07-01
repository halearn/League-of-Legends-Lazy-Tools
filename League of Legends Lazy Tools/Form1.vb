Imports System.Text
Imports System.Net
Imports System.IO
Imports System.Web.Script.Serialization
Imports BitmapDetector2.Search_Image
Imports System.ComponentModel
Imports System.Security.Cryptography
Public Class Form1
    Dim user_ As String
    Dim pass_ As String
    Dim region_ As String
    Dim lolpath_ As String
    Public version As String
    Public upurl As String
    Public upavail As Boolean
#Region "Encrypt"
    Public Function AES_Encrypt(ByVal input As String, ByVal pass As String) As String
        Dim AES As New System.Security.Cryptography.RijndaelManaged
        Dim Hash_AES As New System.Security.Cryptography.MD5CryptoServiceProvider
        Dim encrypted As String = ""

        Dim hash(31) As Byte
        Dim temp As Byte() = Hash_AES.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(pass))
        Array.Copy(temp, 0, hash, 0, 16)
        Array.Copy(temp, 0, hash, 15, 16)
        AES.Key = hash
        AES.Mode = System.Security.Cryptography.CipherMode.ECB
        Dim DESEncrypter As System.Security.Cryptography.ICryptoTransform = AES.CreateEncryptor
        Dim Buffer As Byte() = System.Text.ASCIIEncoding.ASCII.GetBytes(input)
        encrypted = Convert.ToBase64String(DESEncrypter.TransformFinalBlock(Buffer, 0, Buffer.Length))
        Return encrypted

    End Function
#End Region

#Region "Decrypt"
    Public Function AES_Decrypt(ByVal input As String, ByVal pass As String) As String
        Dim AES As New System.Security.Cryptography.RijndaelManaged
        Dim Hash_AES As New System.Security.Cryptography.MD5CryptoServiceProvider
        Dim decrypted As String = ""
        Dim hash(31) As Byte
        Dim temp As Byte() = Hash_AES.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(pass))
        Array.Copy(temp, 0, hash, 0, 16)
        Array.Copy(temp, 0, hash, 15, 16)
        AES.Key = hash
        AES.Mode = System.Security.Cryptography.CipherMode.ECB
        Dim DESDecrypter As System.Security.Cryptography.ICryptoTransform = AES.CreateDecryptor
        Dim Buffer As Byte() = Convert.FromBase64String(input)
        decrypted = System.Text.ASCIIEncoding.ASCII.GetString(DESDecrypter.TransformFinalBlock(Buffer, 0, Buffer.Length))
        Return decrypted
        
    End Function
#End Region
    Private Async Sub checkforupdate()
        Dim client As New WebClient
        Dim url As String = "http://ggwpizi.bugs3.com/LOL2"
        Try

            Dim update As String = New System.Net.WebClient().DownloadString(url)
            Dim ve = update.Split("|")
            version = ve(0)
            upurl = ve(1)
            If version = ProductVersion Then
                upavail = False
            Else
                upavail = True
            End If
            If update = "" Or update = "  " Then
                Dim updatee As String = New System.Net.WebClient().DownloadString("https://docs.google.com/document/export?format=txt&id=1bG0zxwGEty6HpfN_2JAwGbTEeRoBmUU1dr7om9IQJrY")
                Dim vee = update.Split("|")

                If vee(0) = Application.ProductVersion Then
                    upavail = False
                Else
                    upavail = True
                    upurl = vee(1)
                    version = vee(0)
                End If
            Else
                If ve(0) = Application.ProductVersion Then
                    upavail = False
                Else
                    upavail = True
                    upurl = ve(1)
                    version = ve(0)

                End If
            End If

        Catch ex As Exception
            Dim update As String = New System.Net.WebClient().DownloadString("https://docs.google.com/document/export?format=txt&id=1bG0zxwGEty6HpfN_2JAwGbTEeRoBmUU1dr7om9IQJrY")

            Dim ve = update.Split("|")

            If ve(0) = Application.ProductVersion Then
                upavail = False
            Else
                upavail = True
                upurl = ve(1)
                version = ve(0)
            End If
        End Try
    End Sub
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If CheckForInternetConnection() Then  Else MsgBox("No Internet Connection") : Me.Close()
        checkforupdate()
        If upavail Then
            MsgBox("The Program Will Update Now !")
            ' My.Computer.Network.DownloadFile(upurl, Application.StartupPath & "\up.exe", vbNullString, vbNullString, True, 50000, True)

            'Me.Close()
            Dim funkynoise As New System.Net.WebClient
            AddHandler funkynoise.DownloadFileCompleted, AddressOf OnDownloadComplete

            funkynoise.DownloadFileAsync(New Uri(upurl), Application.StartupPath & "\up.exe")

            Exit Sub
        Else
            MsgBox("Program Up-To-Date Version " & version & "    (if you do not see a version number , please go to the thread where you have downloaded the program from to see what note i have left. , or it could only be a temporary problem)")
        End If

        FormSkin1.Text = ("League of Legends Lazy Tools By Saeed Suleiman Version 1.0")
        FlatComboBox1.SelectedIndex = 0
        CheckForIllegalCrossThreadCalls = False
        If My.Computer.FileSystem.FileExists(Application.StartupPath & "\settings.json") Then
            Dim j As New JavaScriptSerializer
            Dim settings As Object = j.Deserialize(Of Settings)(IO.File.ReadAllText(Application.StartupPath & "\settings.json"))
            Dim user As String = settings.username
            Dim pass As String = settings.password
            Dim region As String = settings.region
            Dim lolpath As String = settings.lolpath
            Try
                pass = AES_Decrypt(pass, "_MuchSalt_SoWOW!@#")
            Catch ex As Exception
                MsgBox("Settings Are Corrupted I Will Now Delete The Settings")
                Kill(Application.StartupPath & "/settings.json")
                Application.Restart()

            End Try
            FlatTextBox1.Text = user
            FlatTextBox2.Text = pass
            FlatTextBox3.Text = lolpath
            FlatComboBox1.Text = region
            loaded = True
            status("Settings Loaded")
        Else
            loaded = False
            status("No Settings Found")
            MsgBox("No Settings Found , Please Save New Settings")
            FlatTabControl1.SelectedIndex = 1
        End If
    End Sub
    Private Sub OnDownloadComplete(ByVal sender As Object, ByVal e As AsyncCompletedEventArgs)

        If Not e.Cancelled AndAlso e.Error Is Nothing Then
            MsgBox("Update Downloaded", MsgBoxStyle.Information)
            Process.Start(Application.StartupPath & "\up.exe")
            End
        Else
            MsgBox("Update Download failed", vbCritical)
        End If
    End Sub
    Dim region As String
    Function login(username As String, password As String, region As String)
        Try
            Dim sb As New StringBuilder()
            Dim payload As String = Convert.ToString((Convert.ToString("user=") & username) + ",password=") & password
            Dim query As String = Convert.ToString("payload=") & payload
            Dim loginq As String = region
            Dim con As WebRequest = WebRequest.Create(loginq & Convert.ToString("login-queue/rest/queue/authenticate"))
            con.Method = "POST"
            Dim outputStream As Stream = con.GetRequestStream()
            outputStream.Write(Encoding.ASCII.GetBytes(query), 0, Encoding.ASCII.GetByteCount(query))
            Dim webresponse As WebResponse = con.GetResponse()
            'Dim inputStream As Stream = webresponse.GetResponseStream()
            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function
    Dim p() As Process
    Private Function CheckIfRunning()
        p = Process.GetProcessesByName("LoLLauncher")
        If p.Count > 0 Then
            Return True
        Else
            Return False
        End If
    End Function
    Private Sub FormSkin1_Click(sender As Object, e As EventArgs) Handles FormSkin1.Click

    End Sub

    Private Sub FlatButton1_Click(sender As Object, e As EventArgs) Handles FlatButton1.Click
        OpenFileDialog1.FileName = ""
        OpenFileDialog1.ShowDialog()
    End Sub

    Private Sub OpenFileDialog1_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles OpenFileDialog1.FileOk
        FlatTextBox3.Text = OpenFileDialog1.FileName
    End Sub

    Private Sub FlatButton2_Click(sender As Object, e As EventArgs) Handles FlatButton2.Click
        If Not FlatTextBox1.Text = "" And Not FlatTextBox2.Text = "" And Not FlatTextBox3.Text = "" Then
            If CheckForInternetConnection() = True Then
                FlatButton1.Enabled = False
                FlatButton2.Enabled = False
                FlatButton3.Enabled = False
                back1.RunWorkerAsync()
            Else
                FlatButton1.Enabled = False
                FlatButton2.Enabled = False
                FlatButton3.Enabled = False
                status("No internet")
                MsgBox("There is no internet connection so i can't check if the password & username are correct", MsgBoxStyle.Information, "No Internet")
                save()

            End If
        Else
            status("Error input all please ")

            MsgBox("Please Input All Information", MsgBoxStyle.Critical, "Error")
        End If

    End Sub
    Dim loaded As Boolean
    Sub save()
        Dim settings As Object = New Settings
        settings.username = FlatTextBox1.Text
        settings.password = AES_Encrypt(FlatTextBox2.Text, "add_salt_here_for_encryption") 'obviously not the one i use on the live version.
        settings.region = FlatComboBox1.Text
        settings.lolpath = FlatTextBox3.Text

        Dim j As New JavaScriptSerializer
        Dim json As String = (j.Serialize(settings))
        IO.File.WriteAllText(Application.StartupPath & "\settings.json", json)
        status("Settings Saved")
        MsgBox("Settings Saved")
        loaded = True
        FlatButton1.Enabled = True
        FlatButton2.Enabled = True
        FlatButton3.Enabled = True
    End Sub
    Public Shared Function CheckForInternetConnection() As Boolean
        Try
            Using client = New WebClient()
                Using stream = client.OpenRead("http://www.google.com")
                    Return True
                End Using
            End Using
        Catch
            Return False
        End Try
    End Function
    Private Sub back1_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles back1.DoWork
        Select Case FlatComboBox1.Text
            Case "EUNE"
                region = "https://lq.eun1.lol.riotgames.com/"
            Case "EUW"
                region = "https://lq.eu.lol.riotgames.com/"""
            Case "NA"
                region = "https://lq.na1.lol.riotgames.com/"
            Case "OCE"
                region = "https://lq.oc1.lol.riotgames.com/"
            Case "TR"
                region = "https://lq.tr.lol.riotgames.com/"
            Case "BR"
                region = "https://lq.br.lol.riotgames.com/"
            Case "RU"
                region = "https://lq.ru.lol.riotgames.com/"
            Case "PBE"
                region = "https://lq.pbe1.lol.riotgames.com/"
            Case "LAS"
                region = "https://lq.la2.lol.riotgames.com/"
            Case "LAN"
                region = "https://lq.la1.lol.riotgames.com/"
        End Select
        Dim cor As Boolean
        If login(FlatTextBox1.Text, FlatTextBox2.Text, region) Then
            cor = True
            save()

        Else
            cor = False
            Dim res As DialogResult = MsgBox("Your Login Password & Username Are Incorrect Do You Want To Save Anyway ? ", MsgBoxStyle.YesNo, "Couldn't Get Authkey")
            status("Couldn't Get Authkey")
            If res = Windows.Forms.DialogResult.Yes Then
                'save
                save()
                'end save
            Else
                FlatTextBox1.Text = ""
                FlatTextBox2.Text = ""
            End If
        End If
    End Sub

    Private Sub FlatButton3_Click(sender As Object, e As EventArgs) Handles FlatButton3.Click
        If My.Computer.FileSystem.FileExists(Application.StartupPath & "/settings.json") Then
            Dim j As New JavaScriptSerializer

            Dim settings As Object = j.Deserialize(Of Settings)(IO.File.ReadAllText(Application.StartupPath & "/settings.json"))
            FlatTextBox1.Text = settings.username
            Try
                FlatTextBox2.Text = AES_Decrypt(settings.password, "_MuchSalt_SoWOW!@#")

            Catch
                MsgBox("Settings Corrupted", MsgBoxStyle.Critical)
                Exit Sub
            End Try
            FlatTextBox3.Text = settings.lolpath
            FlatComboBox1.Text = settings.region
            status("Settings Loaded")
            MsgBox("Settings Loaded", MsgBoxStyle.Information, "Settings Loaded")
        Else
            status("No Settings Found")
            MsgBox("No Saved Settings Found", MsgBoxStyle.Exclamation, "No Saved Settings Found")

        End If
    End Sub
    Sub status(ByVal status As String)
        FlatStatusBar1.Text = status & " @ " & My.Computer.Clock.LocalTime
    End Sub
    Private Sub FlatButton4_Click(sender As Object, e As EventArgs) Handles FlatButton4.Click
        Try
            user_ = FlatTextBox1.Text
            pass_ = FlatTextBox2.Text
            region = FlatComboBox1.Text
            lolpath_ = FlatTextBox3.Text
            Process.Start(lolpath_)
            Timer4.Start()
        Catch ex As Exception
            If ex.Message.Contains("file") Then
                MsgBox(ex.Message & " @ " & lolpath_)

            Else
                MsgBox(ex.Message)
            End If

        End Try
    End Sub
    Private Declare Sub mouse_event Lib "user32.dll" (ByVal dwFlags As Integer, ByVal dx As Integer, ByVal dy As Integer, ByVal cButtons As Integer, ByVal dwExtraInfo As IntPtr)
    Private Sub PerformMouseClick(ByVal LClick_RClick_DClick As String)
        Const MOUSEEVENTF_LEFTDOWN As Integer = 2
        Const MOUSEEVENTF_LEFTUP As Integer = 4
        Const MOUSEEVENTF_RIGHTDOWN As Integer = 6
        Const MOUSEEVENTF_RIGHTUP As Integer = 8
        If LClick_RClick_DClick = "RClick" Then
            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, IntPtr.Zero)
            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, IntPtr.Zero)
        ElseIf LClick_RClick_DClick = "LClick" Then
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, IntPtr.Zero)
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, IntPtr.Zero)
        ElseIf LClick_RClick_DClick = "DClick" Then
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, IntPtr.Zero)
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, IntPtr.Zero)
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, IntPtr.Zero)
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, IntPtr.Zero)
        End If
    End Sub






    Private Function PressButton(ByRef image As Bitmap, ByRef press As Boolean, ByRef x As Integer, ByRef y As Integer)
        Dim bounds As Rectangle
        Dim point As New Point
        Dim screenshot As System.Drawing.Bitmap
        Dim graph As Graphics
        bounds = Screen.PrimaryScreen.Bounds
        screenshot = New System.Drawing.Bitmap(bounds.Width, bounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb)
        graph = Graphics.FromImage(screenshot)
        graph.CopyFromScreen(0, 0, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy)
        Dim returnString As Point = BitmapDetector2.Search_Image.search(screenshot, image, 0)
        If returnString.X = 0 And returnString.Y = 0 Then
            Return False
        Else

            If press = True Then
                point.X = returnString.X + x
                point.Y = returnString.Y + y
                Windows.Forms.Cursor.Position = point
                PerformMouseClick("LClick")

            Else
                point.X = returnString.X + x
                point.Y = returnString.Y + y
                Windows.Forms.Cursor.Position = point
            End If

            Return True
        End If
    End Function

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Timer1.Interval = 2000
        If PressButton(My.Resources.launch2, True, 10, 10) Or PressButton(My.Resources.launch, True, 10, 10) Then
            PerformMouseClick("LClick")
            PerformMouseClick("LClick")


            Timer2.Start()
            Timer1.Stop()
            status("Found & Clicked Launch Button")
        Else
            status("Searching For Launch Button")
        End If
    End Sub

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        If PressButton(My.Resources.Main_2, True, 10, 180) Then
            PerformMouseClick("LClick")
            status("Found Main Screen , Typing Username """ & user_ & "")
            SendKeys.Send(user_)
            Timer2.Stop()
            Timer3.Start()
        Else
            status("Searching For Main Screen")
        End If
    End Sub

    Private Sub Timer3_Tick(sender As Object, e As EventArgs) Handles Timer3.Tick
        If PressButton(My.Resources.Main_2, True, 10, 240) Then
            PerformMouseClick("LClick")
            status("Found Main Screen , Typing Password")
            SendKeys.Send(pass_)
            Threading.Thread.Sleep(400)
            SendKeys.Send("{ENTER}")
            Timer3.Stop()
            status("Done !")
        Else
            status("Searching For Main Screen")
        End If
    End Sub

    Private Sub Timer4_Tick(sender As Object, e As EventArgs) Handles Timer4.Tick
        If CheckIfRunning() Then
            Timer5.Start()
            Timer4.Stop()
            status("Lol Detected , Waiting For LOL Client To Fully Load")

        Else
            status("Waiting For League Of Legends To Start")
        End If
    End Sub

    Private Sub Timer5_Tick(sender As Object, e As EventArgs) Handles Timer5.Tick
        If PressButton(My.Resources.online, False, 0, 0) Then
            Timer1.Start()
            Timer5.Stop()
            status("Client is fully Loaded")
        Else
            status("Waiting For Client To Load")
        End If

    End Sub

    Private Sub FlatButton5_Click(sender As Object, e As EventArgs) Handles FlatButton5.Click
        MsgBox("This Feature Will Be Available Next Update", MsgBoxStyle.Information, "Soon TM")
    End Sub

    Private Sub FlatButton6_Click(sender As Object, e As EventArgs) Handles FlatButton6.Click
        Timer1.Stop()
        Timer2.Stop()
        Timer3.Stop()
        Timer4.Stop()
        Timer5.Stop()
        status("Stopped")
    End Sub

    Private Sub Timer6_Tick(sender As Object, e As EventArgs) Handles Timer6.Tick
        If Timer1.Enabled = True Or Timer2.Enabled = True Or Timer3.Enabled = True Or Timer4.Enabled = True Or Timer5.Enabled = True Then
            FlatButton6.Enabled = True
        Else
            FlatButton6.Enabled = False
        End If
    End Sub
End Class
