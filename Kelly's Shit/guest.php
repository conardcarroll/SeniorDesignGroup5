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

$sql = "SELECT UID, Name, Phone_number, Restriction_type FROM user";
$result = $conn->query($sql);
//$UesrID = $row['UID'];
echo "<form>";
if ($result->num_rows > 0) {
     // output data of each row
     while($row = $result->fetch_assoc()) {
         echo "<br> <input type='radio' name='all' id='{$row['UID']}'>: ". $row["UID"]. " - Name: ". $row["Name"]. " " . $row["Phone_number"] . " "  . " " . $row["Restriction_type"] ."<br>";
     }
} else {
     echo "0 results";
}
echo "</form>";
$conn->close();
?>  