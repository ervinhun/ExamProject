export default function Terms() {
    return (
        <div className="container mx-auto px-4 py-8">
            <div className="max-w-4xl mx-auto">
                {/* Header */}
                <div className="mb-8">
                    <h1 className="text-4xl font-bold text-primary mb-3">
                        Terms and Conditions
                    </h1>
                    <p className="text-xl text-base-content/70">
                        Terms for using Jerne Idrætsforening’s website and internal services
                    </p>
                </div>

                {/* Introduction */}
                <div className="prose prose-lg max-w-none mb-8">
                    <p className="text-base-content/80">
                        These Terms and Conditions govern the use of this website and any
                        related internal systems operated by Jerne Idrætsforening
                        (“Jerne IF”, “we”, “us”). By using the website or creating an
                        account, you agree to be bound by these terms.
                    </p>
                </div>

                {/* About the Service */}
                <div className="card bg-base-200 shadow-md mb-8">
                    <div className="card-body">
                        <h2 className="card-title text-2xl mb-4">About the Service</h2>
                        <p className="text-base-content/80">
                            This website is provided to support the activities of
                            Jerne Idrætsforening, including membership administration,
                            internal communication, and club-related services.
                            The services are intended primarily for members and
                            authorized users of the association.
                        </p>
                    </div>
                </div>

                {/* Eligibility */}
                <div className="mb-8">
                    <h2 className="text-2xl font-bold mb-4">Eligibility and Accounts</h2>
                    <div className="space-y-3 text-base-content/80">
                        <p>
                            Access to certain features may require a registered user
                            account and active membership in Jerne IF.
                        </p>
                        <p>
                            You are responsible for ensuring that the information you
                            provide is accurate and up to date, and for maintaining the
                            confidentiality of your login credentials.
                        </p>
                    </div>
                </div>

                {/* Acceptable Use */}
                <div className="card bg-base-200 shadow-md mb-8">
                    <div className="card-body">
                        <h2 className="card-title text-2xl mb-4">Acceptable Use</h2>
                        <div className="space-y-2 text-base-content/80">
                            <p>You agree not to:</p>
                            <ul className="list-disc pl-6 space-y-1">
                                <li>Use the website for unlawful purposes</li>
                                <li>Attempt to gain unauthorized access to systems or data</li>
                                <li>Disrupt or interfere with the operation of the website</li>
                                <li>Provide false or misleading information</li>
                                <li>Misuse the services in a way that harms Jerne IF or other users</li>
                            </ul>
                        </div>
                    </div>
                </div>

                {/* Data & Privacy */}
                <div className="mb-8">
                    <h2 className="text-2xl font-bold mb-4">Data Protection and Privacy</h2>
                    <p className="text-base-content/80">
                        The processing of personal data is governed by our Privacy Policy.
                        Personal data is used solely for internal purposes within
                        Jerne Idrætsforening and is not shared with third parties.
                    </p>
                </div>

                {/* Soft Deletion */}
                <div className="card bg-base-200 shadow-md mb-8">
                    <div className="card-body">
                        <h2 className="card-title text-2xl mb-4">Account Deactivation and Soft Deletion</h2>
                        <p className="text-base-content/80">
                            Jerne IF may deactivate or remove user access if these terms
                            are violated or if an account is no longer required.
                        </p>
                        <p className="text-base-content/80 mt-3">
                            User accounts and related data are subject to a
                            <strong> soft deletion</strong> process. This means data is
                            deactivated and no longer visible in normal use, but may be
                            retained internally for administrative, legal, or
                            documentation purposes.
                        </p>
                    </div>
                </div>

                {/* Availability */}
                <div className="mb-8">
                    <h2 className="text-2xl font-bold mb-4">Availability and Changes</h2>
                    <p className="text-base-content/80">
                        We aim to keep the website available and functioning correctly,
                        but we do not guarantee uninterrupted access. Jerne IF reserves
                        the right to modify, suspend, or discontinue parts of the service
                        at any time.
                    </p>
                </div>

                {/* Liability */}
                <div className="card bg-base-200 shadow-md mb-8">
                    <div className="card-body">
                        <h2 className="card-title text-2xl mb-4">Limitation of Liability</h2>
                        <p className="text-base-content/80">
                            The website and services are provided “as is”.
                            Jerne Idrætsforening cannot be held liable for indirect or
                            consequential damages arising from the use or inability
                            to use the website, to the extent permitted by law.
                        </p>
                    </div>
                </div>

                {/* Governing Law */}
                <div className="mb-8">
                    <h2 className="text-2xl font-bold mb-4">Governing Law</h2>
                    <p className="text-base-content/80">
                        These Terms and Conditions are governed by Danish law.
                        Any disputes shall be subject to the jurisdiction of the
                        Danish courts.
                    </p>
                </div>

                {/* Closing */}
                <div className="text-center py-6">
                    <p className="text-base-content/70">
                        By continuing to use this website, you confirm that you have
                        read and accepted these Terms and Conditions.
                    </p>
                    <p className="text-lg font-semibold text-primary mt-3">
                        Jerne Idrætsforening — fair use for everyone
                    </p>
                </div>
            </div>
        </div>
    );
}
