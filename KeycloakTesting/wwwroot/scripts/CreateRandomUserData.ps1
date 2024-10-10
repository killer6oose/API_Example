# Script: CreateRandomUserData.ps1
# Description: Generates 50 random user records with first and last names covering all 26 letters of the alphabet.
# API Endpoint: http://kctest.cronotech.us/api/UserData

# Function to generate a random full name
function Get-RandomFullName {
    $firstNames = @(
        'Alice', 'Brian', 'Charlotte', 'David', 'Emma', 'Frank', 'Grace', 'Henry', 'Isabella', 'Jack',
        'Katherine', 'Liam', 'Mia', 'Noah', 'Olivia', 'Paul', 'Quinn', 'Ryan', 'Sophia', 'Thomas',
        'Uma', 'Victor', 'Wendy', 'Xander', 'Yvonne', 'Zachary'
    )
    $lastNames = @(
        'Anderson', 'Brown', 'Clark', 'Davis', 'Evans', 'Franklin', 'Garcia', 'Hill', 'Ibrahim', 'Johnson',
        'King', 'Lewis', 'Martinez', 'Nelson', 'O'Neill', 'Perez', 'Quinn', 'Roberts', 'Smith', 'Taylor',
        'Underwood', 'Vasquez', 'Williams', 'Xu', 'Young', 'Zimmerman'
    )

    $firstName = Get-Random -InputObject $firstNames
    $lastName = Get-Random -InputObject $lastNames

    return "$firstName $lastName"
}

# Function to generate a random email address
function Get-RandomEmail {
    $domains = @('example.com', 'test.com', 'demo.com', 'mail.com', 'sample.net')
    $userName = (Get-Random -Minimum 1000 -Maximum 9999).ToString()
    $domain = Get-Random -InputObject $domains

    return "$userName@$domain"
}

# Function to generate a random phone number
function Get-RandomPhoneNumber {
    $areaCode = Get-Random -Minimum 200 -Maximum 999
    $prefix = Get-Random -Minimum 200 -Maximum 999
    $lineNumber = Get-Random -Minimum 1000 -Maximum 9999

    return "($areaCode) $prefix-$lineNumber"
}

# Function to generate a random address
function Get-RandomAddress {
    $streetNumbers = Get-Random -Minimum 100 -Maximum 9999
    $streetNames = @(
        'Apple St', 'Birch St', 'Cherry Ave', 'Dogwood Ln', 'Elm St', 'Fir Rd', 'Grove St', 'Holly Dr',
        'Ivy Ln', 'Juniper Way', 'King St', 'Laurel Ave', 'Maple St', 'Nutmeg Rd', 'Oak St', 'Pine St',
        'Queen St', 'Rose Ave', 'Spruce St', 'Tulip Ln', 'Union St', 'Violet Rd', 'Willow Dr', 'Xenia St',
        'Yucca Ct', 'Zebrawood Ave'
    )
    $cities = @(
        'Albany', 'Boston', 'Chicago', 'Denver', 'Eugene', 'Fresno', 'Greenville', 'Houston', 'Indianapolis',
        'Jacksonville', 'Kansas City', 'Los Angeles', 'Miami', 'New York', 'Orlando', 'Philadelphia',
        'Quincy', 'Raleigh', 'San Francisco', 'Tucson', 'Utica', 'Virginia Beach', 'Washington', 'Xenia',
        'Yonkers', 'Zanesville'
    )
    $states = @('AL', 'AK', 'AZ', 'AR', 'CA', 'CO', 'CT', 'DE', 'FL', 'GA', 'HI', 'ID', 'IL', 'IN', 'IA',
                'KS', 'KY', 'LA', 'ME', 'MD', 'MA', 'MI', 'MN', 'MS', 'MO', 'MT', 'NE', 'NV', 'NH', 'NJ',
                'NM', 'NY', 'NC', 'ND', 'OH', 'OK', 'OR', 'PA', 'RI', 'SC', 'SD', 'TN', 'TX', 'UT', 'VT',
                'VA', 'WA', 'WV', 'WI', 'WY')
    $zip = Get-Random -Minimum 10000 -Maximum 99999

    $streetName = Get-Random -InputObject $streetNames
    $city = Get-Random -InputObject $cities
    $state = Get-Random -InputObject $states

    return "$streetNumbers $streetName, $city, $state $zip"
}

# Function to generate a random access level
function Get-RandomAccessLevel {
    $accessLevels = @('Public', 'Confidential', 'Secret', 'TopSecret')
    return Get-Random -InputObject $accessLevels
}


# Main loop to create and send 50 random user records
$apiUrl = 'http://kctest.cronotech.us/api/UserData'

for ($i = 1; $i -le 50; $i++) {
    # Generate random user data
    $userData = @{
        FullName    = Get-RandomFullName
        UserEmail   = Get-RandomEmail
        PhoneNum    = Get-RandomPhoneNumber
        Address     = Get-RandomAddress
        AccessLevel = Get-RandomAccessLevel
    }

    # Convert the user data to JSON
    $jsonData = $userData | ConvertTo-Json

    # Send the POST request
    try {
        $response = Invoke-RestMethod -Uri $apiUrl -Method POST -Body $jsonData -ContentType 'application/json'

        Write-Host "[$i] User created: $($userData.FullName) - $($userData.UserEmail)"
    } catch {
        Write-Host "[$i] Failed to create user: $($_.Exception.Message)"
    }
}
