  <?php
$servername = "localhost";
$username = "root";
$password = "Group5";
$dbname = "Creechky";

// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);
// Check connection
if ($conn->connect_error) {
     die("Connection failed: " . $conn->connect_error);
} 

$sql = "SELECT UID, Name, Phone_number, Restriction_type FROM user where Restriction_type ='2'";
$result = $conn->query($sql);

if ($result->num_rows > 0) {
     // output data of each row
     while($row = $result->fetch_assoc()) {
         echo "<br> id: ". $row["UID"]. " - Name: ". $row["Name"]. " " . $row["Phone_number"] . " "  . " " . $row["Restriction_type"] ."<br>";
     }
} else {
     echo "0 results";
}

$conn->close();
?>  