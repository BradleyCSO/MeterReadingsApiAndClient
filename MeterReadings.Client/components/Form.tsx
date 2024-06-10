import React, { useState } from 'react';

export const Form = () => {
    const [fileContent, setFileContent] = useState<string | null>(null);
    const [successMessage, setSuccessMessage] = useState<string | null>(null);
    const [errorMessage, setErrorMessage] = useState<string | null>(null);
    
    const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const file = event.target.files?.[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = () => {
                setFileContent(reader.result as string);
            };
            reader.readAsText(file);
        }
    };

    const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();

        if (fileContent) {
            try {
                const response = await fetch('https://localhost:7207/meter-reading-uploads', {
                    method: 'POST',
                    body: fileContent,
                    headers: {
                        'Content-Type': 'text/plain',
                    },
                });

                if (response.ok) {
                    console.log('File uploaded successfully!');
                    setSuccessMessage('File uploaded successfully!');
                } else {
                    console.error('Error uploading file:', response.statusText);
                    setErrorMessage('Error uploading file: ' + response.statusText);
                }
            } catch (error) {
                console.error('Network error:', error);
                setErrorMessage('Network error: ' + error);
            }
        } else {
            setErrorMessage('Please select a file before submitting.');
        }
    };

    return (
        <div>
            <form className='form-container' onSubmit={handleSubmit}>
                <input type='file' accept='.csv' onChange={handleFileChange} />
                <button type='submit'>Upload</button>
            </form>
            {successMessage && <p style={{ textAlign: 'center', color: 'green' }}>{successMessage}</p>}
            {errorMessage && <p style={{ textAlign: 'center', color: 'red' }}>{errorMessage}</p>}
        </div>
    );
};
